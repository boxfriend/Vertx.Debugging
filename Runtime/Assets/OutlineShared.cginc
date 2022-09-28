#include "UnityCG.cginc"
#include "DebuggingShared.cginc"

struct Outine
{
    float3 A, B;
    float3 Radius;
};

StructuredBuffer<Outine> outline_buffer;

struct vertInput
{
    float2 uv : TEXCOORD0;
    uint vertexID : SV_VertexID;
    uint instanceID : SV_InstanceID;
};

struct v2f
{
    float4 position : SV_POSITION;
    float4 color : TEXCOORD1;
};

float2 rotate(float2 v, float a)
{
    float s, c;
    sincos(a, s, c);
    float tx = v.x;
    float ty = v.y;
    v.x = c * tx - s * ty;
    v.y = s * tx + c * ty;
    return v;
}

float2 rotate(float2 v, float s, float c)
{
    float tx = v.x;
    float ty = v.y;
    v.x = c * tx - s * ty;
    v.y = s * tx + c * ty;
    return v;
}

struct Plane
{
    float3 Normal;
    float Distance;
};

bool plane_plane_intersection(
    Plane p1, Plane p2,
    // output args
    out float3 r_point, out float3 r_normal)
{
    // logically the 3rd plane, but we only use the normal component.
    const float3 p3_normal = cross(p1.Normal, p2.Normal);
    const float det = dot(p3_normal, p3_normal);

    // If the determinant is 0, that means parallel planes, no intersection.
    // note: you may want to check against an epsilon value here.
    if (det != 0.0)
    {
        // calculate the final (point, normal)
        r_point = (cross(p3_normal, p2.Normal) * p1.Distance +
            cross(p1.Normal, p3_normal) * p2.Distance) / det;
        r_normal = p3_normal;
        return true;
    }
    return false;
}

int circle_line_intersection(float radius, float2 p, float2 n, out float2 intersection1, out float2 intersection2)
{
    float dx = n.x;
    float dy = n.y;

    float A = dx * dx + dy * dy;
    float B = 2 * (dx * p.x + dy * p.x);
    float C = p.x * p.x + p.y * p.y - radius * radius;

    float det = B * B - 4 * A * C;
    if (A <= 0.0000001 || det < 0)
    {
        // No real solutions.
        intersection1 = float2(0, 0);
        intersection2 = float2(0, 0);
        return 0;
    }
    if (det == 0)
    {
        // One solution.
        float t = -B / (2 * A);
        intersection1 = float2(p.x + t * dx, p.y + t * dy);
        intersection2 = float2(0, 0);
        return 1;
    }

    // Two solutions.
    float t = (-B + sqrt(det)) / (2 * A);
    intersection1 = float2(p.x + t * dx, p.y + t * dy);
    t = (-B - sqrt(det)) / (2 * A);
    intersection2 = float2(p.x + t * dx, p.y + t * dy);
    return 2;
}

bool closest_plane_circle_intersection(
    float3 circleCenter,
    float3 circleNormal,
    float3 circlePerpendicular,
    float circleRadius,
    float3 planePoint,
    float3 planeNormal,
    inout float3 intersection
)
{
    Plane circlePlane;
    circlePlane.Distance = -dot(circleNormal, circleCenter);
    circlePlane.Normal = circleNormal;
    Plane plane;
    plane.Distance = -dot(planeNormal, planePoint);
    plane.Normal = planeNormal;
    float3 pointI, normalI;
    if (!plane_plane_intersection(circlePlane, plane, pointI, normalI))
        return false;
    // zero out positions
    // planePoint -= circleCenter;
    pointI -= circleCenter;
    // project onto circle plane
    float3 circlePerpendicular2 = cross(circleNormal, circlePerpendicular);
    float2 pointILocal = float2(dot(circlePerpendicular, pointI), dot(circlePerpendicular2, pointI));
    float2 normalILocal = float2(dot(circlePerpendicular, normalI), dot(circlePerpendicular2, normalI));

    float2 i1, i2;
    int intersections = circle_line_intersection(circleRadius, pointILocal, normalILocal, i1, i2);
    if (intersections == 0)
        return false;
    if (intersections == 1)
    {
        intersection = circleCenter + circlePerpendicular * i1.x + circlePerpendicular2 * i1.y;
        return true;
    }
    // Reproject back into 3D.
    float3 i13 = circleCenter + circlePerpendicular * i1.x + circlePerpendicular2 * i1.y;
    float3 i23 = circleCenter + circlePerpendicular * i2.x + circlePerpendicular2 * i2.y;
    // Find the closest intersection to our input.
    float d1 = distance(i1, intersection);
    float d2 = distance(i2, intersection);
    intersection = d1 < d2 ? i13 : i23;
    return true;
}

v2f vert(vertInput input)
{
    v2f o;
    Outine outline = outline_buffer[input.instanceID];
    float radius = length(outline.Radius);

    float3 originWorld = input.vertexID == 0 ? outline.A : outline.B;
    float3 direction = normalize(outline.B - outline.A);

    if (is_orthographic())
    {
        float3 right = normalize(cross(-camera_direction(), direction));

        float3 worldPos = originWorld
            + right * radius;
        o.position = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));

        if (has_custom(modifications_buffer[input.instanceID]))
        {
            if (dot(outline.Radius, right) < 0)
            {
                o.color = float4(color_buffer[input.instanceID].xyz, 0);
                return o;
            }
        }

        o.color = color_buffer[input.instanceID];
        return o;
    }
    else
    {
        float3 normal = normalize(_WorldSpaceCameraPos.xyz - originWorld);
        float3 right = normalize(cross(normal, direction));

        float newRadius, offset;
        float3 oNormal;
        get_circle_info(originWorld, radius, newRadius, offset, oNormal);

        // Find the intersection between this new plane (the one created by get_circle_info)
        // and the circle that is at originWorld, facing in direction, and of radius;
        // it's at the closest intersection that we should position the line vertex.
        // This is not at all performant, but it's only 2 vertices, right? RIGHT?
        // Look, this took me forever, give me a break.
        float3 intersection = originWorld + right * radius;

        float alphaMultiplier =
            closest_plane_circle_intersection(
                originWorld,
                direction,
                right,
                radius,
                originWorld + oNormal * offset,
                oNormal,
                intersection
            )
                ? 1
                : 0;
        if (has_custom(modifications_buffer[input.instanceID]))
        {
            if (dot(outline.Radius, intersection - originWorld) < 0)
                alphaMultiplier = 0;
        }
        o.color = float4(color_buffer[input.instanceID].xyz, color_buffer[input.instanceID].a * alphaMultiplier);
        o.position = mul(UNITY_MATRIX_VP, float4(intersection, 1.0));
        return o;
    }
}
