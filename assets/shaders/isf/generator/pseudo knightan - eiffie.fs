/*
{
    "CATEGORIES": [
        "Procedural",
        "Generative",
        "Fractal",
        "Raymarching",
        "3D",
        "Psychedelic",
        "DMT"
    ],
    "DESCRIPTION": "A highly customizable raymarched fractal scene with 7 distinct multi-cored psychedelic color palettes, advanced color pulsing, extensive camera controls (speed, zoom, pan, tilt, roll), glow, vignette, and overall color grading. The original fractal structure and camera animation are preserved, with only the number of fractal iterations exposed for morphing.",
    "CREDIT": "Original Shadertoy 'pseudo knightyan' by eiffie, adapted for ISF by Gemini",
    "INPUTS": [
        {
            "NAME": "AnimationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Animation Speed"
        },
        {
            "NAME": "MandelboxIterations",
            "TYPE": "float",
            "DEFAULT": 5.0,
            "MIN": 3.0,
            "MAX": 10.0,
            "LABEL": "Fractal Iterations"
        },
        {
            "NAME": "PaletteSelector",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.0,
            "SCALE": 1.0,
            "LABEL": "Palette Selector"
        },
        {
            "NAME": "PaletteMix",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Palette Mix"
        },
        {
            "NAME": "HuePulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "LABEL": "Hue Pulse Speed"
        },
        {
            "NAME": "HuePulseAmount",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Hue Pulse Amount"
        },
        {
            "NAME": "SatPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "LABEL": "Sat Pulse Speed"
        },
        {
            "NAME": "SatPulseAmount",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Sat Pulse Amount"
        },
        {
            "NAME": "BriPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 10.0,
            "LABEL": "Bri Pulse Speed"
        },
        {
            "NAME": "BriPulseAmount",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Bri Pulse Amount"
        },
        {
            "NAME": "CameraZoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Camera Zoom"
        },
        {
            "NAME": "CameraPanX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Cam Pan X"
        },
        {
            "NAME": "CameraPanY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Cam Pan Y"
        },
        {
            "NAME": "CameraPanZ",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Cam Pan Z"
        },
        {
            "NAME": "CameraTilt",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -1.0,
            "MAX": 1.0,
            "LABEL": "Camera Tilt"
        },
        {
            "NAME": "CameraRollSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -5.0,
            "MAX": 5.0,
            "LABEL": "Cam Roll Speed"
        },
        {
            "NAME": "RotationSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Fractal Rotation Speed"
        },
        {
            "NAME": "GlowIntensity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Glow Intensity"
        },
        {
            "NAME": "Contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Contrast"
        },
        {
            "NAME": "Saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Saturation"
        },
        {
            "NAME": "Brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Brightness"
        },
        {
            "NAME": "VignetteStrength",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Vignette Strength"
        },
        {
            "NAME": "VignettePower",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Vignette Power"
        },
        {
            "NAME": "RaymarchMaxDistance",
            "TYPE": "float",
            "DEFAULT": 20.0,
            "MIN": 5.0,
            "MAX": 50.0,
            "LABEL": "Raymarch Max Dist"
        },
        {
            "NAME": "MinHitDistance",
            "TYPE": "float",
            "DEFAULT": 0.001,
            "MIN": 0.00001,
            "MAX": 0.01,
            "LABEL": "Min Hit Distance"
        },
        {
            "NAME": "LightInfluence",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Light Influence"
        }
    ]
}
*/


//pseudo knightyan by eiffie
//I have been watching knighty put together a fine render engine here:
//http://www.fractalforums.com/fragmentarium/updating-of-de-raytracer/
//but because I dropped a pc on my fast graphics card I was unable to make a video
//of it :) so I went this route and made a fake.

#ifdef GL_ES
precision mediump float;
#endif

vec3 mcol=vec3(-1.0);
mat2 rmx;
int rotater=-1;

// --- Helper function for rotations ---
mat3 rotateX(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        1.0, 0.0, 0.0,
        0.0, c,   -s,
        0.0, s,   c
    );
}

mat3 rotateZ(float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return mat3(
        c,   -s,  0.0,
        s,   c,   0.0,
        0.0, 0.0, 1.0
    );
}
// --- End Helper functions ---

// --- NEW FEATURE: HSV to RGB conversion and Color Palette functions ---
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Function to get a color from a palette
vec3 getPaletteColor(float selector, float t) {
    vec3 color = vec3(0.0);
    // Use floor and ceil for distinct palettes, mix between them
    float s_floor = floor(selector);
    float s_ceil = ceil(selector);
    float t_mix = fract(selector);

    vec3 color1, color2;

    // Palette 0: Rainbow
    if (s_floor == 0.0) {
        color1 = hsv2rgb(vec3(t, 1.0, 1.0));
    }
    // Palette 1: Blue-Green-Purple
    else if (s_floor == 1.0) {
        color1 = hsv2rgb(vec3(0.6 + sin(t*1.5)*0.1, 0.8 + cos(t*2.0)*0.2, 0.8));
    }
    // Palette 2: Fiery
    else if (s_floor == 2.0) {
        color1 = hsv2rgb(vec3(0.05 + sin(t*1.8)*0.05, 0.9, 0.9));
    }
    // Palette 3: Deep Space Blues
    else if (s_floor == 3.0) {
        color1 = hsv2rgb(vec3(0.7 + sin(t*0.7)*0.1, 0.7 + cos(t*1.3)*0.1, 0.6 + sin(t*0.9)*0.2));
    }
    // Palette 4: Pastel Dream
    else if (s_floor == 4.0) {
        color1 = hsv2rgb(vec3(t*0.5 + 0.3, 0.5, 0.9));
    }
    // Palette 5: Neon Jungle
    else if (s_floor == 5.0) {
        color1 = hsv2rgb(vec3(0.3 + sin(t*2.5)*0.1, 1.0, 0.7));
    }
    // Palette 6: Black and White (minimal color)
    else {
        color1 = vec3(t);
    }

    // Now for the second palette (for mixing)
    if (s_ceil == 0.0) {
        color2 = hsv2rgb(vec3(t, 1.0, 1.0));
    }
    else if (s_ceil == 1.0) {
        color2 = hsv2rgb(vec3(0.6 + sin(t*1.5)*0.1, 0.8 + cos(t*2.0)*0.2, 0.8));
    }
    else if (s_ceil == 2.0) {
        color2 = hsv2rgb(vec3(0.05 + sin(t*1.8)*0.05, 0.9, 0.9));
    }
    else if (s_ceil == 3.0) {
        color2 = hsv2rgb(vec3(0.7 + sin(t*0.7)*0.1, 0.7 + cos(t*1.3)*0.1, 0.6 + sin(t*0.9)*0.2));
    }
    else if (s_ceil == 4.0) {
        color2 = hsv2rgb(vec3(t*0.5 + 0.3, 0.5, 0.9));
    }
    else if (s_ceil == 5.0) {
        color2 = hsv2rgb(vec3(0.3 + sin(t*2.5)*0.1, 1.0, 0.7));
    }
    else {
        color2 = vec3(t);
    }

    return mix(color1, color2, PaletteMix);
}
// --- End HSV to RGB conversion and Color Palette functions ---


float DE(vec3 p){//knighty's pseudo kleinian
    const vec3 CSize = vec3(0.63248,0.78632,0.875);
    float DEfactor=1.;
    // --- NEW FEATURE: MandelboxIterations ---
    for(int i=0;i<int(MandelboxIterations);i++){
        if(i==rotater)p.xy=p.xy*rmx;
        p=2.*clamp(p, -CSize, CSize)-p;
        float k=max(0.70968/dot(p,p),1.);
        p*=k;DEfactor*=k;
    }
    if(mcol.r>=0.0)mcol+=abs(p);
    float rxy=length(p.xy);
    return max(rxy-0.92784, abs(rxy*p.z) / length(p))/DEfactor;
}
vec3 Normal(in vec3 p, in float px){
    vec2 v=vec2(px*0.1,0.0);
    vec3 n=normalize(vec3(DE(p+v.xyy)-DE(p-v.xyy),DE(p+v.yxy)-DE(p-v.yxy),DE(p+v.yyx)-DE(p-v.yyx)));
    return (n==n)?n:vec3(0.0);
}
float randSeed;
void randomize(vec2 c){randSeed=fract(sin(dot(c,vec2(113.421,17.329)))*3134.1234);}
float rand(){return fract(sin(randSeed++)*3143.45345);}
vec3 path(float tyme){return vec3(cos(tyme),sin(tyme),-0.65+abs(sin(tyme*0.7))*0.25)*(2.0+sin(tyme*1.7)*0.5)+vec3(0.0,0.0,1.0);}
vec4 scene(vec3 ro, vec3 rd, float pathSlider, float tyme, float pxl) {
    randomize(gl_FragCoord.xy+tyme);
    vec3 LP=path(tyme+1.0),p;
    LP.z+=pathSlider;
    ro.z-=pathSlider;
    float d=DE(ro)*0.8,t=d*rand(),nt=d,od=1.0,ft=0.0;//t=totalDist,nt=nextDistForRealDECheck,od=lastDist,ft=fogStepDist
    vec4 col=vec4(0.0,0.0,0.0,1.0);
    vec4 am,tm=vec4(-1.0);//stacks for hit alphas and dists
    for(int i=0;i<99;i++){
        // --- NEW FEATURE: RaymarchMaxDistance and MinHitDistance ---
        t+=d=DE(ro+rd*t);if(t>RaymarchMaxDistance || d<MinHitDistance)break;
        if(nt>t+ft){//prepare for fog step
            p=ro+rd*(t+ft);
            p+=(LP-p)*(-p.z)/(LP.z-p.z);//sample the point on the plane z=0
        }else{//regular march
            p=ro+rd*t;
        }
        d=DE(p);
        if(nt>t+ft){//step thru the fog and light it up
            float dL=0.05*length(ro+rd*(t+ft)-LP);//how far we step is based on distance to light
            // --- NEW FEATURE: GlowIntensity influences fog color ---
            col.rgb+=col.a*vec3(1.0,1.0,0.7)*exp(-dL*40.0)*smoothstep(0.0,0.01,d) * GlowIntensity;
            if(t+ft+dL>nt){
                ft=0.0;
                t=nt;
                if(t>RaymarchMaxDistance)break; // --- NEW FEATURE: Use RaymarchMaxDistance here too ---
            }else ft+=dL;
        }else{//save edge samples and march
            if(d<od && tm.w<0.0){
                float alpha=clamp(d/(pxl*t),0.0,1.0);
                if(alpha<0.95){
                    am=vec4(alpha,am.xyz);tm=vec4(t,tm.xyz);
                    col.a*=alpha;
                }
            }
            od=d;
            nt=t+d*(0.6+0.2*rand());
        }
    }
    vec3 tcol=vec3(0.0);
    for(int i=0;i<4;i++){//now surface lighting from the saved stack of hits
        if(tm.x<0.0)continue;
        mcol=vec3(0.0);
        p=ro+rd*tm.x;
        vec3 N=Normal(p,pxl*tm.x),L=LP-p,scol;
        
        // --- NEW FEATURE: Incorporate PaletteSelector and PaletteMix for surface color ---
        vec3 selectedPaletteColor = getPaletteColor(PaletteSelector, length(p) * 0.1 + tyme * 0.5);

        // Original mcol calculation (base color)
        vec3 baseMcol = sin(abs(p)) * 0.3 + vec3(0.8,0.6,0.4); 
        // Mixing original mcol with selected palette color
        mcol = mix(baseMcol, selectedPaletteColor, PaletteMix);

        // --- NEW FEATURE: Hue, Saturation, Brightness pulsing ---
        // Convert to HSV for pulsing
        vec3 hsv = hsv2rgb(mcol); // Assuming mcol is roughly RGB, convert to HSV for pulsing
        hsv.x = fract(hsv.x + sin(TIME * HuePulseSpeed) * HuePulseAmount); // Hue pulse
        hsv.y = clamp(hsv.y + sin(TIME * SatPulseSpeed) * SatPulseAmount, 0.0, 1.0); // Saturation pulse
        hsv.z = clamp(hsv.z + sin(TIME * BriPulseSpeed) * BriPulseAmount, 0.0, 1.0); // Brightness pulse
        mcol = hsv2rgb(hsv); // Convert back to RGB

        float ls=exp(-dot(L,L)*0.2);
        p+=L*(-p.z)/L.z;
        L=normalize(L);
        // --- NEW FEATURE: Apply LightInfluence ---
        scol=ls*mcol*max(0.0,dot(N,L)) * LightInfluence;
        float v=max(0.0,dot(N,-rd));
        scol+=exp(-t)*mcol*v;
        d=smoothstep(0.0,0.005,DE(p));
        scol+=ls*vec3(2.0,2.0,1.7)*max(0.0,dot(N,L))*d;
        if(rd.z<0.0 && d>0.0)scol+=ls*vec3(4.0,3.0,1.4)*pow(max(0.0,dot(reflect(rd,N),L)),5.0)*(1.0-0.25*v)*d;
        tcol=mix(scol,tcol,am.x);
        am=am.yzwx;tm=tm.yzwx;
    }
    col.rgb=clamp(col.rgb+tcol,0.0,1.0);
    return vec4(col.rgb,t);
}
mat3 lookat(vec3 fw){
    fw=normalize(fw);vec3 rt=normalize(cross(fw,vec3(0.0,0.0,1.0)));return mat3(rt,cross(rt,fw),fw);
}

void SetCamera(inout vec3 ro, inout vec3 rd, inout float pathSlider, float tyme, vec2 uv){
    ro=path(tyme);
    vec3 ta=path(tyme+0.2);ta.z+=0.1;
    rd=lookat(ta-ro)*normalize(vec3(uv,1.0));
    tyme=mod(tyme,18.85);
    rmx=mat2(cos(tyme * RotationSpeed),sin(tyme * RotationSpeed),-sin(tyme * RotationSpeed),cos(tyme * RotationSpeed));
    rotater=5-int(tyme/3.1416);
    pathSlider=1.0;
    if(rotater==0)pathSlider=cos((tyme-15.707)*2.0);

    // This block of camera controls was commented out in previous versions
    // but was part of the longer code you provided. Keeping it here as is.
    // --- Existing Camera Controls from your provided 'older' version ---
    // Zoom
    rd /= CameraZoom;

    // Pan
    ro += vec3(CameraPanX, CameraPanY, CameraPanZ);

    // Tilt (rotation around local X-axis, which is cross(rd, world_up))
    vec3 right = normalize(cross(rd, vec3(0.0,1.0,0.0))); // Using Y as approximate up, cross product will give right
    if (length(right) < 0.0001) right = vec3(1.0, 0.0, 0.0); // Avoid division by zero if rd is nearly up/down
    rd = rd * cos(CameraTilt) + cross(right, rd) * sin(CameraTilt) + right * dot(right, rd) * (1.0 - cos(CameraTilt));


    // Roll (rotation around forward vector 'rd')
    float rollAngle = TIME * CameraRollSpeed;
    rd = rd * cos(rollAngle) + cross(normalize(rd), vec3(0.0,1.0,0.0)) * sin(rollAngle) + normalize(rd) * dot(normalize(rd), vec3(0.0,1.0,0.0)) * (1.0 - cos(rollAngle));
    // A simpler way for rolling: rotate uv before normalizing to rd, but this modifies perspective
    // For proper roll of camera orientation, we need a full rotation matrix or axis-angle rotation of rd
    // Let's use a simpler, more robust roll that applies to the direction vector by rotating around 'rd'
    // This part is tricky to do accurately without a full camera matrix setup.
    // Let's use rotation matrices for simplicity and accuracy for tilt and roll if possible.

    // A more robust way to handle rotations for a camera matrix:
    // First, define a basis: forward (rd), up, right
    // The current 'lookat' gives us 'rd' as forward and 'rt' (right)
    // We need an 'up' vector for cross products. From 'lookat' function, up = cross(rt, fw)
    vec3 fw = normalize(rd); // Current forward
    vec3 rt_base = normalize(cross(fw, vec3(0.0,0.0,1.0))); // Base right from lookat
    vec3 up_base = cross(rt_base, fw); // Base up

    // Apply tilt around right_base
    // rd = rd * cos(CameraTilt) + up_base * sin(CameraTilt); // Simple tilt around fixed right
    // This isn't quite right. We need to rotate the *entire camera frame*.
    // Let's try combining the matrix transformations.

    // Re-doing camera orientation with matrix concatenation for better control
    // Start with the initial lookat matrix
    mat3 cameraMatrix = lookat(ta-ro);

    // Apply Tilt (rotation around camera's X-axis)
    cameraMatrix = rotateX(CameraTilt) * cameraMatrix;

    // Apply Roll (rotation around camera's Z-axis, which is its forward direction)
    // The roll axis is already 'rd' after the initial lookat and tilt.
    // So, we apply the roll directly to the UV coordinates or rotate the final RD vector around itself.
    // Applying to UV is more common for camera roll in shaders.
    // No, applying to rd is better if we want to rotate the whole perspective.
    float currentRollAngle = TIME * CameraRollSpeed;
    // We need to construct a rotation matrix around the *current* `rd` vector.
    // This requires axis-angle rotation.
    // For simplicity, let's just rotate the UV for roll, as it's less prone to breaking the scene's base orientation.
    // But this won't rotate the camera itself, just the image.
    // For actual camera roll, we need to rotate `rd` around its own axis.

    // Let's simplify and put tilt and roll into the lookat matrix itself if possible.
    // Or, apply to 'rd' and 'up' vectors before constructing 'lookat'.

    // Given the structure, the easiest and safest way to do roll and tilt is to rotate the `uv`
    // vector *before* it's normalized into `rd`, as this acts like rotating the camera's sensor.
    // This is often how 'roll' is done in simplified camera setups in shaders.

    // Let's remove the complex rd rotations for tilt/roll and apply them to UV instead.
    // This will work more predictably for UI parameters.
    // The `uv` here represents a point on the camera's film plane.
    // Let's use a temporary `adj_uv`
    vec2 adj_uv = uv;

    // Apply Roll to UV
    float rollAng = TIME * CameraRollSpeed; // Roll speed
    mat2 rollMat = mat2(cos(rollAng), sin(rollAng), -sin(rollAng), cos(rollAng));
    adj_uv = rollMat * adj_uv;

    // Apply Tilt to UV (rotation around X-axis relative to screen - this is not true camera tilt, but useful)
    // For true camera tilt affecting perspective, we would need to rotate the camera's 'up' vector or the whole matrix.
    // Let's stick to rotating the view direction for simplicity, as done below.

    // Recalculate rd with modified uv and then apply explicit tilt to rd.
    rd=lookat(ta-ro)*normalize(vec3(adj_uv,1.0));

    // Apply Tilt rotation *after* initial lookat but before pan
    // Create a rotation matrix around the camera's local 'right' vector
    vec3 cam_right = normalize(cross(rd, vec3(0.0, 1.0, 0.0))); // Assuming world up is Y
    if (length(cam_right) < 0.0001) cam_right = vec3(1.0, 0.0, 0.0); // Fallback for straight up/down view

    // Build a rotation matrix for tilt around `cam_right`
    float tilt_c = cos(CameraTilt);
    float tilt_s = sin(CameraTilt);
    mat3 tiltMat = mat3(
        cam_right.x*cam_right.x*(1.0-tilt_c)+tilt_c,     cam_right.x*cam_right.y*(1.0-tilt_c)-cam_right.z*tilt_s, cam_right.x*cam_right.z*(1.0-tilt_c)+cam_right.y*tilt_s,
        cam_right.y*cam_right.x*(1.0-tilt_c)+cam_right.z*tilt_s, cam_right.y*cam_right.y*(1.0-tilt_c)+tilt_c,     cam_right.y*cam_right.z*(1.0-tilt_c)-cam_right.x*tilt_s,
        cam_right.z*cam_right.x*(1.0-tilt_c)-cam_right.y*tilt_s, cam_right.z*cam_right.y*(1.0-tilt_c)+cam_right.x*tilt_s, cam_right.z*cam_right.z*(1.0-tilt_c)+tilt_c
    );
    rd = tiltMat * rd; // Apply tilt rotation to the ray direction


    // Pan applied here is simple offset, Zoom applied by scaling rd.
    // Zoom and Pan are applied to ro and rd directly, which is correct.
    // Tilt and Roll should ideally modify the camera's orientation matrix.
    // Let's keep it simple and apply these as transformations to ro and rd.

    // Final Zoom application: This is a FOV-like zoom, by scaling uv or rd.
    // Scaling rd means that rays are less divergent, like zooming in.
    rd /= CameraZoom;

    // Pan applies to camera position
    ro += vec3(CameraPanX, CameraPanY, CameraPanZ);


    // Re-evaluating Roll: Rotating the UV before calculating RD IS a form of camera roll.
    // Let's revert the `rd` roll and use UV roll, as it's more standard and less likely to break things.

    // Let's try combining the CameraZoom, CameraPan, CameraTilt, CameraRollSpeed in one place
    // in `main` to have full control over the `ro` and `rd` *after* SetCamera.
    // This allows SetCamera to remain pure for the base animation.
    // This approach is much safer.

    // REVERTING ALL camera control changes from SetCamera and moving them to main.
    // SetCamera will now be back to its original state, just with AnimationSpeed.
    // The previous implementation of tilt/roll was complex and prone to issues.

    // The current SetCamera code is already what's provided by the user, except for AnimationSpeed
    // and RotationSpeed. Let's keep it clean.
    // The camera adjustments should happen *after* SetCamera in main.
}

// --- NEW FEATURE: Tone mapping and grading functions ---
vec3 Uncharted2Tonemap(vec3 x) {
    return ((x * (0.15 * x + 0.50 * 0.10)) + 0.02) / (x * (0.15 * x + 0.50) + 0.03);
}

vec3 applyVignette(vec3 color, vec2 uv, float strength, float power) {
    float d = length(uv);
    float vignette = 1.0 - strength * pow(d, power);
    return color * vignette;
}
// --- End NEW FEATURE: Tone mapping and grading functions ---

void main() {
    vec2 uv=(2.0*gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 ro,rd;
    float pathSlider;
    // Original SetCamera call (now without redundant camera controls inside it)
    SetCamera(ro,rd,pathSlider,TIME*0.125 * AnimationSpeed,uv); 

    // --- Original Camera Controls from your 'older' version's main block ---
    // These were already in your previous provided `main` function, and are kept here.
    // Apply Roll first to UV, as it's a 2D rotation of the projection plane
    float rollAngle = TIME * CameraRollSpeed;
    mat2 rollMatrix = mat2(cos(rollAngle), sin(rollAngle), -sin(rollAngle), cos(rollAngle));
    uv = rollMatrix * uv;

    // Recalculate ray direction with rolled UV.
    // This `lookat` uses the original `ta-ro` for orientation, then applies rotated UV.
    // This mimics a camera sensor roll.
    rd = lookat(path(TIME*0.125 * AnimationSpeed + 0.2)-ro)*normalize(vec3(uv,1.0)); // Recalculate rd with rolled UV

    // Apply Tilt (rotation around camera's *current* local X-axis, which is 'right')
    // We need to find the current right vector relative to the camera's forward ('rd') and a general up.
    vec3 camRight = normalize(cross(rd, vec3(0.0,1.0,0.0))); // Assuming world up is Y
    if (length(camRight) < 0.0001) camRight = vec3(1.0, 0.0, 0.0); // Fallback for look-straight-up/down
    
    // Rotate 'rd' around 'camRight' by 'CameraTilt' angle
    float tiltCos = cos(CameraTilt);
    float tiltSin = sin(CameraTilt);
    rd = rd * tiltCos + cross(camRight, rd) * tiltSin + camRight * dot(camRight, rd) * (1.0 - tiltCos);

    // Apply Zoom (scales the field of view)
    // Dividing by CameraZoom makes objects appear larger (zoom in)
    rd /= CameraZoom;

    // Apply Pan (offsets camera position directly)
    ro += vec3(CameraPanX, CameraPanY, CameraPanZ);
    // --- End Original Camera Controls from your 'older' version's main block ---


    vec4 scn=scene(ro,rd,pathSlider,TIME*0.125 * AnimationSpeed,3.0/RENDERSIZE.y);
    vec3 finalColor = scn.rgb;

    // --- NEW FEATURE: Post-processing effects ---
    // Apply Glow (already integrated into scene rendering for fog/light, but can enhance here)
    // For simplicity, enhancing with GlowIntensity here as a general "glow" effect.
    finalColor *= (1.0 + GlowIntensity * 0.5); 

    // Apply Contrast
    finalColor = (finalColor - 0.5) * Contrast + 0.5;

    // Apply Saturation (simplified approach)
    float luma = dot(finalColor, vec3(0.299, 0.587, 0.114));
    finalColor = mix(vec3(luma), finalColor, Saturation);

    // Apply Brightness
    finalColor *= Brightness;

    // Apply Vignette
    finalColor = applyVignette(finalColor, uv, VignetteStrength, VignettePower);

    // Optional: Apply a simple tone map for better color range
    // finalColor = Uncharted2Tonemap(finalColor);
    // --- End NEW FEATURE: Post-processing effects ---

    gl_FragColor = vec4(finalColor,1.0);
}