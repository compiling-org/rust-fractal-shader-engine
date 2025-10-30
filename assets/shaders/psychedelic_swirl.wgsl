// Psychedelic Swirl - Audio-reactive shader
@group(0) @binding(0) var<uniform> time: f32;
@group(0) @binding(1) var<uniform> resolution: vec2<f32>;
@group(0) @binding(2) var<uniform> mouse: vec2<f32>;
@group(0) @binding(3) var<uniform> audio_level: f32;

@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    var color = vec3<f32>(0.0);
    
    let center = vec2<f32>(0.5);
    let dist = distance(uv, center);
    
    // Swirl pattern
    let angle = atan2(uv.y - center.y, uv.x - center.x);
    let radius = dist * (2.0 + audio_level);
    let swirl = angle + radius * 3.0 + time * 2.0;
    
    // Color calculation
    let r = sin(swirl + time * 0.5) * 0.5 + 0.5;
    let g = sin(swirl + time * 0.7 + 2.0) * 0.5 + 0.5;
    let b = sin(swirl + time * 0.3 + 4.0) * 0.5 + 0.5;
    
    color = vec3<f32>(r, g, b);
    
    // Apply audio reactivity
    color *= 0.5 + audio_level * 0.5;
    
    return vec4<f32>(color, 1.0);
}