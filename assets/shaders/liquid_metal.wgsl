// Liquid Metal Shader - Metallic liquid flow effect
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> flow_speed: f32;
@group(0) @binding(3) var<uniform> metallicity: f32;
@group(0) @binding(4) var<uniform> turbulence: f32;

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    var color = vec3<f32>(0.0);
    
    // Liquid flow pattern
    let flow = vec2<f32>(
        sin(uv.y * 10.0 + time * flow_speed),
        cos(uv.x * 8.0 + time * flow_speed * 0.7)
    );
    
    // Add turbulence
    let noise = sin((uv.x + uv.y) * 20.0 + time) * turbulence;
    let distorted_uv = uv + flow * 0.1 + noise * 0.05;
    
    // Metallic shading
    let normal = vec3<f32>(
        sin(distorted_uv.x * 5.0),
        cos(distorted_uv.y * 5.0),
        1.0
    );
    
    let light = normalize(vec3<f32>(1.0, 1.0, 0.5));
    let intensity = max(dot(normalize(normal), light), 0.0);
    
    // Metal colors
    let base_color = vec3<f32>(0.8, 0.8, 0.9);
    let highlight = vec3<f32>(1.0, 1.0, 1.0);
    
    color = mix(base_color, highlight, intensity * metallicity);
    
    return vec4<f32>(color, 1.0);
}