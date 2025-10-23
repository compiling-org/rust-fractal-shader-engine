/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy",
        "Landscape",
        "Procedural",
        "Psychedelic",
        "Trippy"
    ],
    "DESCRIPTION": "Automatically converted and enhanced from https://www.shadertoy.com/view/XltSRf by Dave_Hoskins. Distance field pylons, in grass. Following a Catmull-Rom spline, with some added effects. Now with custom controls for pylon properties, environment colors, lightning, rain, and a wide array of psychedelic color palettes and post-processing effects. All user input from mouse and audio has been removed for autonomous animation.",
    "IMPORTED": {
        "tex1": {
            "NAME": "tex1",
            "PATH": "92d7758c402f0927011ca8d0a7e40251439fba3a1dac26f5b8b62026323501aa.jpg"
        },
        "tex2": {
            "NAME": "tex2",
            "PATH": "f735bee5b64ef98879dc618b016ecf7939a5756040c2cde21ccb15e69a6e1cfb.png"
        },
        "tex3": {
            "NAME": "tex3",
            "PATH": "f735bee5b64ef98879dc618b016ecf7939a5756040c2cde21ccb15e69a6e1cfb.png"
        }
    },
    "INPUTS": [
        {
            "NAME": "pylonDensity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the density/spacing of the pylons."
        },
        {
            "NAME": "pylonMorph",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Morphs the shape of the individual pylon segments."
        },
        {
            "NAME": "grassColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.05, 0.0, 1.0],
            "DESCRIPTION": "Tint color for the grass."
        },
        {
            "NAME": "skyColor",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.1, 0.085, 1.0],
            "DESCRIPTION": "Tint color for the sky/fog."
        },
        {
            "NAME": "lightningFrequency",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Controls how often lightning flashes occur. Higher is more frequent."
        },
        {
            "NAME": "lightningIntensity",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the brightness of the lightning flashes."
        },
        {
            "NAME": "rainStrength",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the visibility and impact of the rain on the camera."
        },
        {
            "NAME": "cameraMotionSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Multiplies the overall speed of the camera movement along the spline."
        },
        {
            "NAME": "zoomFactor",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "STEP": 0.01,
            "DESCRIPTION": "Controls the camera's zoom level. Higher values zoom in."
        },
        {
            "NAME": "colorPalette",
            "TYPE": "int",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 6,
            "STEP": 1,
            "DESCRIPTION": "Selects one of 7 psychedelic color palettes.",
            "PRAGMA": "COLOR_PALETTE_ENUM"
        },
        {
            "NAME": "colorPulseSpeed",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "STEP": 0.1,
            "DESCRIPTION": "Speed of color pulsing effect."
        },
        {
            "NAME": "brightness",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall brightness."
        },
        {
            "NAME": "saturation",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall color saturation."
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "STEP": 0.01,
            "DESCRIPTION": "Adjusts the overall image contrast."
        }
    ],
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferB"
        },
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferC"
        },
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferD"
        },
        {
        }
    ]
}
*/

// Movement...
// #define FLY_CAMERA // Keep this commented out unless you want free camera movement

// * * CONTROLS * *
// Camera movement is now fully automated along the spline.

#define NUM_POINTS 61

#define INVERT_Y 0
#define ACCEL .01
#define DECAY  .85 // how much velocity is preserved per frame (proportionally)
#define MAX_SPEED  .1
#define TAU 6.28318530718


#if INVERT_Y
const float yMul = 1.0;
#else
const float yMul = -1.0;
#endif

vec3 camPos[NUM_POINTS];

// --- Explicitly declare uniforms here ---
uniform int colorPalette; // <--- ADDED THIS LINE

// --- End explicit uniform declarations ---


//----------------------------------------------------------------------------------------
// Removed ReadKey function as there's no audio input for keys anymore.

//----------------------------------------------------------------------------------------
float Scale = 4.;
float MinRad2 = 0.25;
float sr = 4.0;
vec3 fo =vec3 (0.7,.9528,.9);
vec3 gh = vec3 (.8,.7,0.5638);
vec3 gw = vec3 (.3, 0.5 ,.2);

#ifndef FLY_CAMERA

// Catmull-rom spline
vec3 spline(vec3 p0, vec3 p1, vec3 p2, vec3 p3, float t){

    vec3 c2 = -.5 * p0    + 0.5*p2;
    vec3 c3 = p0        + -2.5*p1 + 2.0*p2 + -.5*p3;
    vec3 c4 = -.5 * p0    + 1.5*p1 + -1.5*p2 + 0.5*p3;
    return(((c4 * t + c3) * t + c2) * t + p1);
}

//-----------------------------------------------------------------------------------------------------------
vec3 getPosAtTime(float t)
{
    int i = int(t);
    uvec4 id = uvec4(i-1, i, i+1, i+2);
    id %= uint(camPos.length());
    vec3 p0 = camPos[id.x];
    vec3 p1 = camPos[id.y];
    vec3 p2 = camPos[id.z];
    vec3 p3 = camPos[id.w];


    return spline(p0, p1, p2, p3, fract(t));

}

#endif
//-----------------------------------------------------------------------------------------------------------
// Pylon, by Dave Hoskins
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

//-------------------------------------------------------------------------------------------------
#define SUN_COLOUR vec3(1., .95, .8)
#define HASHSCALE .1031

float gTime = 0.0; // This `gTime` is a local variable to control specific animations, not the global `TIME`
float movement = 0.0;
vec2 id_pylon;
vec3 sunLight;

// Removed SEE_NUMBERS related functions as they are no longer necessary without user interaction.

//-------------------------------------------------------------------------------------------------
// From https://www.shadertoy.com/view/4djSRW
float Hash( float p )
{
    vec2 p2 = fract(vec2(p) * HASHSCALE);
    p2 += dot(p2.yx, p2.xy+19.19);
    return fract(p2.x * p2.y);
}
float Hash(vec2 p)
{
    vec3 p3  = fract(vec3(p.xyx) * HASHSCALE);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}
//----------------------------------------------------------------------------------------
float Noise( in vec3 x )
{
    vec3 p = floor(x);
    vec3 f = fract(x);
    f = f*f*(3.0-2.0*f);

    vec2 uv = (p.xy+vec2(37.0,17.0)*p.z) + f.xy;
    vec2 rg = IMG_NORM_PIXEL(tex2, (uv+ 0.5)/256.0).yx;
    return mix( rg.x, rg.y, f.z );
}
vec3 GetSky(vec3 pos, vec3 customSkyColor)
{
    pos *= 4.;
    pos.y += TIME;
    float t = Noise(pos);
    t += Noise(pos * 2.1) * .5;
    t += Noise(pos * 4.3) * .25;
    t += Noise(pos * 7.9) * .125;
    return (t) * customSkyColor + vec3(.1,.1,.1);
}

//----------------------------------------------------------------------------------------
float Noise( in vec2 x )
{
    x *= 100.;
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f*f*(3.0-2.0*f);

    float res = mix(mix( Hash(p), Hash(p+ vec2(1.0, 0.0)),f.x),
                    mix( Hash(p+ vec2(.0, 1.0)), Hash(p+ vec2(1.0, 1.0)),f.x),f.y);
    return res;
}
//----------------------------------------------------------------------------------------
float box(vec2 p, vec2 b) {
    vec2 d = abs(p) - b;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

//----------------------------------------------------------------------------------------
mat2 rotMat(float a)
{
    float s = sin(a);
    float c = cos(a);

    return mat2(c, s, -s, c);
}

//----------------------------------------------------------------------------------------
float de(vec3 p, float pylonDensity, float pylonMorph)
{
    p*= pylonDensity;
    vec4 q = vec4(p, 1);
    q.xyz = q.xzy;
    id_pylon = floor(2.5+q.xy/16.0);
    q.xy = mod(q.xy + 8., 16.0) -8.;
    q.z -= 8.0;
    float m = 0.0;

    m = gTime*5.9;
    m *= movement*smoothstep(-.2, .2, q.z);

    mat2 m1 = rotMat(-(.3+sin(id_pylon.y-m)*.05-.08));
    mat2 m2 = rotMat(-.4*(1.+sin(id_pylon.x)*.4));

    for(int i = 0; i < 6; i++)
    {
        q.xyz = abs(q.xyz + 1.) - 1.5;
        q *= 1.5;
        q.xz = m1 * q.xz;
        q.zy = m2 * q.zy;
    }

    float f = box(q.zy, vec2(1.4) * pylonMorph)/q.w;

    return f;
}

//----------------------------------------------------------------------------------------
vec3 texCube( sampler2D sam, in vec3 p, in vec3 n )
{
    vec3 x = IMG_NORM_PIXEL(tex1, p.yz).xzy;
    vec3 y = IMG_NORM_PIXEL(tex1, p.zx).xyz;
    vec3 z = IMG_NORM_PIXEL(tex1, p.xy).yzx;
    vec3 col = (x*abs(n.x) + y*abs(n.y) + z*abs(n.z))/(abs(n.x)+abs(n.y)+abs(n.z));

    return col;
}

// Helper functions for HSV conversion (moved here to be defined before use)
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * mix(K.xxx, clamp(p - K.x, 0.0, 1.0), c.y);
}

//----------------------------------------------------------------------------------------
// Function to apply different color palettes
// Removed paletteIndex parameter, will directly use global colorPalette uniform
vec3 applyPalette(vec3 color) {
    vec3 newColor = color;
    float t = TIME * colorPulseSpeed;

    // Simple HSV conversion for pulsing
    vec3 hsv = rgb2hsv(color);
    hsv.x = mod(hsv.x + t * 0.1, 1.0); // Hue shift
    hsv.y = hsv.y * (1.0 + 0.2 * sin(t * 2.0)); // Saturation pulse
    hsv.z = hsv.z * (1.0 + 0.1 * cos(t * 3.0)); // Brightness pulse

    vec3 pulsedColor = hsv2rgb(hsv);

    switch(colorPalette) { // Directly use the global colorPalette uniform
        case 0: // Original + Pulsing
            newColor = pulsedColor;
            break;
        case 1: // Neon Dream
            newColor = mix(pulsedColor, vec3(0.0, 1.0, 0.5), 0.3 * sin(TIME * 0.5));
            newColor = mix(newColor, vec3(0.8, 0.2, 1.0), 0.3 * cos(TIME * 0.7));
            break;
        case 2: // Cyberpunk Haze
            newColor = mix(pulsedColor, vec3(0.0, 0.8, 0.8), 0.4 * sin(TIME * 0.6));
            newColor = mix(newColor, vec3(1.0, 0.4, 0.0), 0.4 * cos(TIME * 0.8));
            break;
        case 3: // Acid Trip
            newColor = mix(pulsedColor, vec3(0.9, 1.0, 0.0), 0.5 * sin(TIME * 0.9));
            newColor = mix(newColor, vec3(0.0, 0.9, 0.1), 0.5 * cos(TIME * 1.1));
            break;
        case 4: // Deep Space
            newColor = mix(pulsedColor, vec3(0.1, 0.0, 0.5), 0.4 * sin(TIME * 0.4));
            newColor = mix(newColor, vec3(0.7, 0.1, 0.8), 0.4 * cos(TIME * 0.6));
            break;
        case 5: // Electric Rainbow
            newColor = mix(pulsedColor, vec3(sin(TIME*2.0)*0.5+0.5, cos(TIME*2.5)*0.5+0.5, sin(TIME*3.0)*0.5+0.5), 0.6);
            break;
        case 6: // Monochromatic Glitch
            newColor = mix(pulsedColor, vec3(sin(TIME * 5.0) * 0.1 + 0.5), 0.7);
            newColor.r = mix(newColor.r, newColor.g, 0.1 * sin(TIME * 10.0));
            newColor.b = mix(newColor.b, newColor.r, 0.1 * cos(TIME * 12.0));
            break;
    }
    return newColor;
}


// Removed colorPalette parameter from the signature
vec4 colour(vec3 p, vec3 nor, float ty, vec3 customGrassColor)
{
    vec4 col;
    if (ty == 0.0)
    {
        col.xyz = texCube(tex1, p.xyz*.1, nor);
        col.xyz = pow(col.xyz, vec3(2.))*1.;
        float f = col.x+col.y+col.z;
        if (f < .2) col.x = mix(col.x, 1., smoothstep(0.0, .02, f)*abs(sin(TIME*(.8+movement)-p.y*.1)));

        col = col *.75 +.2;
        col.y = abs(sin(id_pylon.y*2.+id_pylon.x*2.)*col.y);
        col.z = abs(cos(id_pylon.y*id_pylon.x)*col.z);
        col.w = .3;
    }
    else
    {
        col.xyz = (1.0-IMG_NORM_PIXEL(tex1,mod(p.xz*.01,1.0)).yzx)*.2+customGrassColor;
        col.w = .04;
    }
    col.xyz = applyPalette(col.xyz); // Call applyPalette without the colorPalette argument
    return col;
}


//----------------------------------------------------------------------------------------
float Terrain( in vec2 p)
{
    float type = 0.0;
    vec2 pos = p*0.00025;
    float w = 10.0;
    float f = -4.0;
    for (int i = 0; i < 4; i++)
    {
        f += Noise(pos) * w;
        w = w * 0.52;
        pos *= 2.;
    }
    return f;
}
//----------------------------------------------------------------------------------------
vec2 map(in vec3 p, float pylonDensity, float pylonMorph)
{
    float r = 0.0;
    float h = p.y-Terrain(p.xz);
    if (h < 3.0) r= pow(Noise(p.xz*.2),2.2)*Noise(p*.007)*3.;// ...Grass
    h -= r;
    float d = de(p, pylonDensity, pylonMorph);
    if (h < d)
        return vec2(h, 1.0 + h);
    else
        return vec2(d, 0.0);
}

//----------------------------------------------------------------------------------------
float mapShad(in vec3 p, float pylonDensity, float pylonMorph)
{
    float h = p.y-Terrain(p.xz);
    float d = de(p, pylonDensity, pylonMorph);
    return min(h, d);
}

//----------------------------------------------------------------------------------------
float Shadow( in vec3 ro, in vec3 rd, float pylonDensity, float pylonMorph)
{
    float res = 1.0;
    float t = .1;
    float h;

    for (int i = 0; i < 7; i++)
    {
        vec3 p =  ro + rd*t;
        h = mapShad(p, pylonDensity, pylonMorph);
        res = min(4.*h / t, res);
        t += h;
    }
    return max(res, .2);
}

//----------------------------------------------------------------------------------------
vec3 DoLighting(in vec4 mat, in vec3 pos, in vec3 normal, in vec3 eyeDir, in float d, in float sh)
{
    vec3 col = mat.xyz * SUN_COLOUR*(max(dot(sunLight,normal), 0.0)) * sh;
    col += mat.xyz *(max(dot(-sunLight,normal), 0.0));

    normal = reflect(eyeDir, normal); // Specular...
    col += pow(max(dot(sunLight, normal), 0.0), 10.0)  * SUN_COLOUR * mat.w *sh;
    // Abmient..
    col += .04 * max(normal.y, 0.1);


    return col;
}

//----------------------------------------------------------------------------------------
vec3 GetNormal(vec3 p, float sphereR, float pylonDensity, float pylonMorph)
{
    vec2 eps = vec2(sphereR, 0.0);
    return normalize( vec3(
            mapShad(p+eps.xyy, pylonDensity, pylonMorph) - mapShad(p-eps.xyy, pylonDensity, pylonMorph),
            mapShad(p+eps.yxy, pylonDensity, pylonMorph) - mapShad(p-eps.yxy, pylonDensity, pylonMorph),
            mapShad(p+eps.yyx, pylonDensity, pylonMorph) - mapShad(p-eps.yyx, pylonDensity, pylonMorph) ) );
}

//----------------------------------------------------------------------------------------
float Scene(in vec3 rO, in vec3 rD, in float t, float pylonDensity, float pylonMorph)
{

    vec3 p;
    for( int j=0; j < 100; j++ )
    {
        if (t > 150.0) break;
        p = rO + t*rD;
        vec2 de = map(p, pylonDensity, pylonMorph);
        if(de.x < .01)
        {
            break;
        }

        t += de.x*.9;
    }
    return t;
}

//----------------------------------------------------------------------------------------
// Rain on the 'camera'...
vec2 camRot;
//----------------------------------------------------------------------------------------

float Noise_tex3( in vec3 x )
{
    vec3 p = floor(x);
    vec3 f = fract(x);
    f = f*f*(3.0-2.0*f);

    vec2 uv = (p.xy+vec2(37.0,17.0)*p.z) + f.xy;
    vec2 rg = IMG_NORM_PIXEL(tex3, (uv+ 0.5)/256.0).yx;
    return mix( rg.x, rg.y, f.z );
}
//----------------------------------------------------------------------------------------

float fbm(vec2 p, float rainStrength)
{
    p *= vec2(24, 8.);
    p.y+=TIME*.7;
    vec3 q =vec3(p, TIME);
      float f = 0.0;
    float a = 1.;
    for (int i = 0; i < 3; i++)
    {
        f += Noise_tex3(q) * a;
        a *= .5;
        q *= 2.;
    }
    // Scale the effect of rain based on rainStrength
    return clamp(f-.6+smoothstep(-10., 60., camRot.x)*.85 ,0.0, .04) * rainStrength;
}

//----------------------------------------------------------------------------------------

// Pylon by David Hoskins.
// License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// It's meant to be some kind of defunct power grid or something, I don't really know.

// 'Buf A' for manual/auto camera stuff
// 'Buf B' does the main rendering / text info.
// 'Buf C' for rain blobs on the camera.
// 'Buf D' for temporal anti-aliasing.
// 'Image' does the final colouring.

vec3 post(vec3 rgb, vec2 xy, float brightness, float saturation, float contrast)
{
    // Apply brightness, saturation, contrast in order
    rgb = rgb * brightness;

    vec3 luma = vec3(dot(vec3(0.2125, 0.7154, 0.0721), rgb));
    rgb = mix(luma, rgb, saturation);

    rgb = mix(vec3(0.5), rgb, contrast);

    // Vignette...
    rgb *= .5+0.5 *pow(60.0*xy.x*xy.y*(1.0-xy.x)*(1.0-xy.y), 0.5);

    return clamp(rgb, 0.0, 1.0);
}

//----------------------------------------------------------------------------------------
void main() {
    if (PASSINDEX == 0)    {


        gl_FragColor = vec4(0.0,0.0,0.0,1.0);

        #ifdef FLY_CAMERA
        // FLY_CAMERA mode is deprecated without user input controls
        // This block will not be active since it depends on ReadKey and mouseXY which are removed.
        // Keeping it commented out for historical context.
        /*
        if ( int(gl_FragCoord.y) == 0 )
        {
            if ( int(gl_FragCoord.x) == 0 )
            {
                vec3 camPos = IMG_NORM_PIXEL(BufferA,mod(vec2(.5,.5)/RENDERSIZE.xy,1.0),-100.0).xyz;
                vec3 camVel = IMG_NORM_PIXEL(BufferA,mod(vec2(3.5,.5)/RENDERSIZE.xy,1.0),-100.0).xyz;
                float time_diff  = (TIME-IMG_NORM_PIXEL(BufferA,mod(vec2(4.5,.5)/RENDERSIZE.xy,1.0),-100.0).x)*30.0;
                if (FRAMEINDEX == 0)
                {
                    gl_FragColor = vec4(-161.0, 6.8, 507., 0.0);
                }else
                {
                    camVel *= time_diff*(1.0+ReadKey(KEY_SHIFT)+ReadKey(KEY_SPACE));
                    vec3 oldCam = camPos;
                    camPos += camVel;
                    gl_FragColor = vec4(camPos, 0);
                }
            }
            else if ( int(gl_FragCoord.x) <= 2 )
            {
                vec4 baseCamRot = IMG_NORM_PIXEL(BufferA,mod(vec2(2.5,.5)/RENDERSIZE.xy,1.0),-100.0);
                vec4 camRot = IMG_NORM_PIXEL(BufferA,mod(vec2(1.5,.5)/RENDERSIZE.xy,1.0),-100.0);

                vec2 mouseRot = (mouseXY.yx/RENDERSIZE.yx-.5)*vec2(.5*yMul,1.) *360.0;

                camRot.w = mouseXY.z;

                bool press = (camRot.w > .0);
                bool lastPress = (baseCamRot.w > .0);
                bool click = press && !lastPress;
                if ( click )
                {
                    baseCamRot.xy -= mouseRot;
                }

                if ( press )
                {
                    camRot.xy = baseCamRot.xy + mouseRot;
                }
                else
                {
                    baseCamRot = camRot;
                }

                baseCamRot.w = camRot.w;

                if ( int(gl_FragCoord.x) == 1 )
                {
                    if (FRAMEINDEX == 0)
                    {
                        gl_FragColor = vec4(.0, .75, .0,0);
                    }else
                    gl_FragColor = camRot;
                }
                else
                {
                    gl_FragColor = baseCamRot;
                }
            }
            else if ( int(gl_FragCoord.x) == 3 )
            {
                vec4 camVel = IMG_NORM_PIXEL(BufferA,mod(vec2(3.5,.5)/RENDERSIZE.xy,1.0),-100.0);
                vec4 camRot = IMG_NORM_PIXEL(BufferA,mod(vec2(1.5,.5)/RENDERSIZE.xy,1.0),-100.0);

                camRot*= TAU/360.;

                vec3 forward = vec3(0,0,ACCEL);
                vec3 right     = vec3(ACCEL,0,0);

                forward.zy = forward.zy*cos(camRot.x) + sin(camRot.x)*vec2(1,-1)*forward.yz;
                right.zy = right.zy*cos(camRot.x) + sin(camRot.x)*vec2(1,-1)*right.yz;

                forward.xz = forward.xz*cos(camRot.y) + sin(camRot.y)*vec2(1,-1)*forward.zx;
                right.xz = right.xz*cos(camRot.y) + sin(camRot.y)*vec2(1,-1)*right.zx;

                camVel.xyz += (ReadKey(KEY_W)-ReadKey(KEY_S)+ReadKey(KEY_UP)-ReadKey(KEY_DOWN)) * forward;
                camVel.xyz += (ReadKey(KEY_D)-ReadKey(KEY_A)+ReadKey(KEY_RIGHT)-ReadKey(KEY_LEFT)) * right;


                camVel *= DECAY;
                float lim = length(camVel);
                if (lim > MAX_SPEED)
                {
                    camVel = normalize(camVel) * MAX_SPEED;
                }


                gl_FragColor = camVel;
            }else if ( int(gl_FragCoord.x) == 4 )
            {
                gl_FragColor = vec4(TIME, 0,0,0);
            */

        #else
        // Catmull-rom spline coords...
        camPos[0] = vec3(-161.5,24.3,505.7);
        camPos[1] = vec3(-161.7, 21.5, 489.3);
        camPos[2] = vec3(-161.6, 22., 485.5);
        camPos[3] = vec3(-162.2, 22.4, 480.8);
        camPos[4] = vec3(-161.4, 20.5, 472.1);
        camPos[5] = vec3(-161.0, 16.5, 466.);
        camPos[6] = vec3(-162.6, 9.3, 457.3);
        camPos[7] = vec3(-161.9, 6.0, 445.7);
        camPos[8] = vec3(-161.7, 9.3, 437.);
        camPos[9] = vec3(-166.6, 12.4, 432.2);
        camPos[10] = vec3(-175.3, 13.1, 426.2);
        camPos[11] = vec3(-179.4, 22.0, 454.7);
        camPos[12] = vec3(-166.5, 20.1, 464.9);
        camPos[13] = vec3(-161.0, 18.0, 465.4);
        camPos[14] = vec3(-151.3, 13.9, 465.);
        camPos[15] = vec3(-135.8, 10.4, 465.7);
        camPos[16] = vec3(-131.8, 6.2, 473.7);
        camPos[17] = vec3(-131.7, 6.9, 485.8);
        camPos[18] = vec3(-130.7, 16.2, 499.8);
        camPos[19] = vec3(-130.1, 21.9, 508.6);
        camPos[20] = vec3(-130.0, 26.8, 537.8);
        camPos[21] = vec3(-130.6, 36.1, 564.5);
        camPos[22] = vec3(-141.5, 26.8, 590.0);
        camPos[23] = vec3(-154.7, 15.8, 584.2);
        camPos[24] = vec3(-161.5, 12.4, 583.7);
        camPos[25] = vec3(-166.6, 12.6, 581.6);
        camPos[26] = vec3(-163.6, 13.2, 587.4);
        camPos[27] = vec3(-159.6, 13.2, 587.7);
        camPos[28] = vec3(-153.6, 13.5, 588.3);
        camPos[29] = vec3(-137.2, 13.4, 591.0);
        camPos[30] = vec3(-121.8, 12.8, 585.7);
        camPos[31] = vec3(-116.8, 11.3, 576.4);
        camPos[32] = vec3(-109., 6.6, 556.7);
        camPos[33] = vec3(-106.5, 3.9, 552.);
        camPos[34] = vec3(-103.7, 3.9, 548.9);
        camPos[35] = vec3(-102.9, 4.3, 545.6);
        camPos[36] = vec3(-102.7, 5.9, 542.0);
        camPos[37] = vec3(-103.1, 5.4, 538.2);
        camPos[38] = vec3(-104.1, 6.5, 535.6);
        camPos[39] = vec3(-105.5, 8.2, 532.6);
        camPos[40] = vec3(-110.1, 11.5, 524.5);
        camPos[41] = vec3(-126.1, 14.4, 522.5);
        camPos[42] = vec3(-144.2, 12.9, 513.4);
        camPos[43] = vec3(-133.2, 9.6, 533.0);
        camPos[44] = vec3(-131.7, 8.0, 541.1);
        camPos[45] = vec3(-125.7, 4.8, 556.5);
        camPos[46] = vec3(-130.0, 5.6, 571.7);
        camPos[47] = vec3(-129.6, 6.9, 580.7);
        camPos[48] = vec3(-129.8, 18.6, 594.0);
        camPos[49] = vec3(-130.1, 25.5, 600.6);
        camPos[50] = vec3(-132.0, 41.1, 610.8);
        camPos[51] = vec3(-132.0, 45.7, 654.6);
        camPos[52] = vec3(-145.0, 24.6, 663.5);
        camPos[53] = vec3(-143.0, 13.1, 656.9);
        camPos[54] = vec3(-140.0, 8.9, 605.2);
        camPos[55] = vec3(-138.0, 8.3, 582.1);
        camPos[56] = vec3(-136.8, 6.8, 576.4);
        camPos[57] = vec3(-138.8, 6.6, 568.6);
        camPos[58] = vec3(-147.8, 10.6, 540.7);
        camPos[59] = vec3(-155.8, 17.6, 520.7);
        camPos[60] = vec3(-161.9, 19.5, 513.9);

        float t = TIME * cameraMotionSpeed * .5;

        if ( int(gl_FragCoord.x) <= 1)
        {
            vec3 pos = getPosAtTime(t);
            if ( int(gl_FragCoord.x) == 0)
            {
                gl_FragColor = vec4(pos, 1.0);
            }else
            if ( int(gl_FragCoord.x) == 1)
            {
                // Get look at target
                vec3 tar1 = getPosAtTime(t+.1);
                vec3 tar = getPosAtTime(t+.2);
                tar = (tar+tar1)*.5;
                pos = tar - pos;
                // Euler rotation angles...
                vec3 rot = vec3(0);
                rot.y = (atan(pos.x, pos.z))*360./TAU;
                rot.x = (clamp(-pos.y*.5, -.8, 1.))*360./TAU-3.141;
                gl_FragColor = vec4(rot,0);

            }
        }
        else if ( int(gl_FragCoord.x) == 6)
        {
            vec3 pos = getPosAtTime(t+1.);
            gl_FragColor = vec4(pos,0);
        }
        else if ( int(gl_FragCoord.x) == 7)
        {
            gl_FragColor = vec4(t,0,0,0);
        }

        #endif

    }
    else if (PASSINDEX == 1)    {



        vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 uv = (-1. + 2.0 * xy) * vec2(RENDERSIZE.x/RENDERSIZE.y,1.0) / zoomFactor;
        sunLight  = normalize(vec3(  .8, .8,  0.8 ));

        vec3 cameraPos = IMG_NORM_PIXEL(BufferA,mod(vec2(.5,.5)/RENDERSIZE.xy,1.0),-100.0).xyz;
        vec4 camRot    = IMG_NORM_PIXEL(BufferA,mod(vec2(1.5,.5)/RENDERSIZE.xy,1.0),-100.0);
        vec4 box       = IMG_NORM_PIXEL(BufferA,mod(vec2(6.5,.5)/RENDERSIZE.xy,1.0),-100.0);
        gTime         = IMG_NORM_PIXEL(BufferA,mod(vec2(7.5,.5)/RENDERSIZE.xy,1.0),-100.0).x+5.;
        vec4 cam = camRot * TAU/ 360.0;
        vec3 col = vec3(.3);
        movement = smoothstep(8.0, 16.0, gTime) * smoothstep(60.0, 50.0, gTime);


        vec3 dir = normalize( vec3(uv, sqrt(max(1. - dot(uv.xy, uv.xy)*.1, 0.))));

        float roll = .05 * sin(TIME*.3);
        dir.xy = dir.xy*cos(roll) + sin(roll)*vec2(1,-1)*dir.yx;
        dir.zy = dir.zy*cos(cam.x) + sin(cam.x)*vec2(1,-1)*dir.yz;
        dir.xz = dir.xz*cos(cam.y) + sin(cam.y)*vec2(1,-1)*dir.zx;

        float lightning_val = 0.0;
        float f_time = mod(gTime * lightningFrequency, 15.);
        if (f_time < .8)
        {
            f_time = smoothstep(.44, .0, f_time)* 1.5;
            lightning_val = mod(-gTime*(1.5-Hash(gTime*2.8)*.001), 1.0) * f_time * lightningIntensity;
        }
        vec3 flash = clamp(vec3(.4, .3, .8) * lightning_val, 0.0, 1.0)*.5;
        dir.y += lightning_val*.1;

        float dis = Scene(cameraPos, dir, 01.* Hash(gl_FragCoord.xy), pylonDensity, pylonMorph);
        vec3 sky = GetSky(dir, skyColor.rgb);

        vec3 normal;
        float sha;
        if (dis  < 100.0)
        {
            vec3 pos = cameraPos + dir * dis;
            sha = Shadow(pos, sunLight, pylonDensity, pylonMorph);

            vec2 de = map(pos, pylonDensity, pylonMorph);
            float f_noise = Noise(pos*vec3(30.0, 2.0, 30.0)+vec3(0.0, gTime*14.0, 0.0))+.4;
            normal = GetNormal(pos, .01, pylonDensity, pylonMorph);
            // Removed colorPalette argument from the colour function call
            vec4 alb =    colour(pos, normal, de.y, grassColor.rgb);

                alb *= f_noise;

            vec3 mat = DoLighting(alb, pos, normal, dir, dis, sha);
            col = mix(sky,mat, clamp(exp(-dis*dis*.0004),0.0, 1.0));

        }else
        {
            col = sky;
        }

        col += pow(max(dot(sunLight, dir), 0.0), 1.3)  * SUN_COLOUR * .05;

            // Rain & Lightning together...
        vec2 st =  uv * vec2(1.8, .04)+vec2(TIME*.01-uv.y*.2, TIME*.4+camRot.x*31.);
        // I'm adding two parts of the texture to stop repetition...
        // Removed audioInput from this line.
        f_time = max(IMG_NORM_PIXEL(tex2,mod(st*.1,1.0)).y, IMG_NORM_PIXEL(tex2,mod(st*.05,1.0)).x);
        f_time = clamp(pow(abs(f_time), 10.0) * 1.0, 0.0, xy.y);
        col = mix(col, skyColor.rgb*.2, f_time);
        col+= lightning_val*.12;


        col = col * smoothstep(.0, 2.0, TIME)+flash;

        gl_FragColor=vec4(col, 1.);
    }
    else if (PASSINDEX == 2)    {


        vec3 col;
        // Removed mouseXY dependence from m calculation. Animation is now purely time-based.
        float m = (TIME / 10.0) * 20.0; // Adjusted for automatic animation
        float gTime = ((TIME)*.2+m);
        vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy;
        camRot = IMG_NORM_PIXEL(BufferA,mod(vec2(1.5,.5)/RENDERSIZE.xy,1.0),-100.0).xy;

        vec2 cam = -camRot * TAU/ 360.0;
        vec3 dir = vec3(0,0,1);
        dir.zy = dir.zy*cos(cam.x) + sin(cam.x)*vec2(1,-1)*dir.yz;
        dir.xz = dir.xz*cos(cam.y) + sin(cam.y)*vec2(1,-1)*dir.zx;

        // Pass rainStrength to fbm function
        vec3 nor      = vec3(0.0,         fbm(xy, rainStrength), 0.0);
        vec3 v2        = nor-vec3(.002,    fbm(xy+vec2(.002,0.0), rainStrength), 0.0);
        vec3 v3        = nor-vec3(0.0,    fbm(xy+vec2(0.0,.002), rainStrength), .002);
        nor = cross(v2, v3);
        nor = normalize(nor);

        vec2 off = xy+vec2(sin(nor.x)*.03,nor.z*.03);
        col = IMG_NORM_PIXEL(BufferB,mod(off,1.0)).xyz;
        col += max(dot(nor, dir), 0.0)*.01;

        gl_FragColor=vec4(col, 1.);
    }
    else if (PASSINDEX == 3)    {


        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec4 lastColour  = IMG_NORM_PIXEL(BufferD,mod(uv,1.0));
        vec4 newColour = IMG_NORM_PIXEL(BufferC,mod(uv,1.0));
        float blur = (TIMEDELTA/.07);

        newColour = newColour * blur + lastColour * (1.0-blur);

        gl_FragColor = newColour;
    }
    else if (PASSINDEX == 4)    {



        vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
        vec4 finalColor = IMG_NORM_PIXEL(BufferD,mod(p,1.0));
        finalColor.rgb = post(finalColor.rgb, p, brightness, saturation, contrast);
        finalColor = pow(finalColor, vec4(0.45));

        gl_FragColor = finalColor;
    }

}