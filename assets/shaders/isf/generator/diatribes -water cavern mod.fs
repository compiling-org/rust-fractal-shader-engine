/*{
  "DESCRIPTION": "Fractal Tunnel - Faithful recreation of the Shadertoy effect",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Tunnel", "Psychedelic"],
  "INPUTS": [
    {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0},
    {"NAME": "depth", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0},
    {"NAME": "intensity", "TYPE": "float", "DEFAULT": 300.0, "MIN": 100.0, "MAX": 1000.0},
    {"NAME": "color_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5},
    {"NAME": "symmetry", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0, "STEP": 1.0},
    {"NAME": "rotation_speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "pulse_rate", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0}
  ]
}*/

/*{
  "DESCRIPTION": "Fractal Tunnel - Faithful recreation of the Shadertoy effect",
  "ISFVSN": "2.0",
  "CATEGORIES": ["Fractal", "Tunnel", "Psychedelic"],
  "INPUTS": [
    {"NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0},
    {"NAME": "depth", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0},
    {"NAME": "intensity", "TYPE": "float", "DEFAULT": 300.0, "MIN": 100.0, "MAX": 1000.0},
    {"NAME": "color_speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.01, "MAX": 0.5},
    {"NAME": "symmetry", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0, "STEP": 1.0},
    {"NAME": "rotation_speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "pulse_rate", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0}
  ]
}*/

vec3 tanh(vec3 x) {
    return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
}

vec3 palette(float t) {
    return 0.5 + 0.5 * cos(6.28318 * (t + vec3(0.0, 0.33, 0.67)));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float time = TIME * speed;
    
    // Kaleidoscopic symmetry
    float angle = atan(uv.y, uv.x);
    float radius = length(uv);
    angle = mod(angle, 6.28318 / symmetry);
    uv = vec2(cos(angle), sin(angle)) * radius;
    
    // Rotation
    float rot = time * rotation_speed;
    uv *= mat2(cos(rot), -sin(rot), sin(rot), cos(rot));
    
    // Tunnel effect
    vec3 col = vec3(0.0);
    float z = depth; // Using depth parameter
    
    for (int i = 0; i < 90; i++) {
        vec3 p = vec3(uv * 2.0, z);
        p.z += 6.0;
        
        // Fractal distortion
        float dj = 1.0;
        for (int j = 0; j < 8; j++) {
            p += cos(p.yzx * dj - vec3(time, time * 0.5, 0.0)) / dj;
            dj /= 0.8;
        }
        
        z += 0.01 + 0.1 * length(p.xz);
        float color_t = sin(z * color_speed + time);
        col += palette(color_t) * 0.01 / (0.1 + length(p.xy));
    }
    
    // Pulse effect
    float pulse = 1.0 + 0.5 * sin(time * pulse_rate);
    gl_FragColor = vec4(tanh(col * intensity * pulse), 1.0);
}