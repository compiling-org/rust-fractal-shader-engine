/*{
  "CATEGORIES": ["Psychedelic", "Geometry", "Transform", "Chaos"],
  "DESCRIPTION": "Extensive psychedelic fractal with multiple geometry types, many color palettes, and controls.",
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "BufferA",
      "PERSISTENT": true
    }
  ],
  "INPUTS": [
    {"NAME": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5},
    {"NAME": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3},
    {"NAME": "TransformMode", "TYPE": "float", "DEFAULT": 1, "MIN": 0, "MAX": 5, "VALUES": [{"NAME":"Basic", "VALUE":0},{"NAME":"Symmetric", "VALUE":1},{"NAME":"Chaos", "VALUE":2},{"NAME":"MultiFractal", "VALUE":3},{"NAME":"Swirl", "VALUE":4},{"NAME":"Spiral", "VALUE":5}]},
    {"NAME": "GeometryType", "TYPE": "float", "DEFAULT": 1, "MIN": 0, "MAX": 6, "VALUES": [{"NAME":"Sphere", "VALUE":0},{"NAME":"Torus", "VALUE":1},{"NAME":"Mandelbox", "VALUE":2},{"NAME":"Sierpinski", "VALUE":3},{"NAME":"Chaos", "VALUE":4},{"NAME":"Klein Bottle", "VALUE":5},{"NAME":"Octahedron", "VALUE":6}]},
    {"NAME": "ChaosIntensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0, "MAX": 2},
    {"NAME": "ChaosSpeed", "TYPE": "float", "DEFAULT": 0.20, "MIN": 0.1, "MAX": 4},
    {"NAME": "ColorPaletteMode", "TYPE": "float", "DEFAULT": 14, "MIN": 0, "MAX": 19, "VALUES": [
      {"NAME":"Vibrant", "VALUE":0},{"NAME":"Psycho", "VALUE":1},{"NAME":"Pastel", "VALUE":2},{"NAME":"Neon", "VALUE":3},{"NAME":"Deep", "VALUE":4},
      {"NAME":"Sunset", "VALUE":5},{"NAME":"Ocean", "VALUE":6},{"NAME":"Night", "VALUE":7},{"NAME":"Retro", "VALUE":8},
      {"NAME":"Fire", "VALUE":9},{"NAME":"Ice", "VALUE":10},{"NAME":"Galaxy", "VALUE":11},{"NAME":"Aurora", "VALUE":12},
      {"NAME":"Rainforest", "VALUE":13},{"NAME":"Desert", "VALUE":14},{"NAME":"Vintage", "VALUE":15},{"NAME":"Electric", "VALUE":16},
      {"NAME":"Frost", "VALUE":17},{"NAME":"Sunrise", "VALUE":18},{"NAME":"Dusk", "VALUE":19}
    ]},
    {"NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0, "MAX": 3},
    {"NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3},
    {"NAME": "FeedbackAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0, "MAX": 1},
    {"NAME": "Glow", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0, "MAX": 2},
    {"NAME": "Symmetry", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0, "MAX": 4},
    {"NAME": "ChaosMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0, "MAX": 1}
  ]
}*/

// Define constants
#define PI 3.141592
#define MAX_STEPS 128
#define BAILOUT 16.0

// Rotation matrix for 2D
mat2 rot2(float a) {
    float c= cos(a);
    float s= sin(a);
    return mat2(c,s,-s,c);
}

// Palette function
vec3 pal(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

// Color palettes
vec3 getColorPalette(int mode, float t) {
    if (mode==0) return pal(t, vec3(0.8,0.2,1.0), vec3(1.0,0.5,0.2), vec3(1.0,1.0,0.2), vec3(0.1,0.2,0.5)); // Vibrant
    if (mode==1) return pal(t, vec3(0.9,0.1,0.3), vec3(0.3,0.9,0.2), vec3(0.2,0.4,0.9), vec3(1.0,0.2,0.9)); // Psycho
    if (mode==2) return pal(t, vec3(0.2,0.8,0.9), vec3(0.9,0.4,0.1), vec3(0.5,0.1,0.7), vec3(1.0,1.0,0.5)); // Pastel
    if (mode==3) return pal(t, vec3(1.0,0.0,0.5), vec3(0.0,1.0,0.5), vec3(0.5,0.0,1.0), vec3(1.0,1.0,1.0)); // Neon
    if (mode==4) return pal(t, vec3(0.3,0.6,0.9), vec3(0.9,0.3,0.6), vec3(0.6,0.9,0.3), vec3(1.0,0.7,0.2)); // Deep
    if (mode==5) return pal(t, vec3(1.0,0.5,0.2), vec3(0.2,0.8,0.5), vec3(0.8,0.2,0.5), vec3(0.5,0.5,0.5)); // Sunset
    if (mode==6) return pal(t, vec3(0.0,0.5,1.0), vec3(1.0,0.0,0.5), vec3(0.5,1.0,0.0), vec3(0.2,0.4,0.6)); // Ocean
    if (mode==7) return pal(t, vec3(0.1,0.1,0.3), vec3(0.3,0.1,0.1), vec3(0.1,0.3,0.1), vec3(0.6,0.6,0.6)); // Night
    if (mode==8) return pal(t, vec3(1.0,0.8,0.2), vec3(0.2,1.0,0.8), vec3(0.8,0.2,1.0), vec3(0.4,0.2,0.6)); // Retro
    if (mode==9) return pal(t, vec3(1.0,0.3,0.0), vec3(0.0,1.0,0.3), vec3(0.3,0.0,1.0), vec3(0.2,0.4,0.6)); // Fire
    if (mode==10) return pal(t, vec3(0.0,0.5,1.0), vec3(1.0,1.0,0.0), vec3(0.5,0.0,0.0), vec3(0.2,0.4,0.6)); // Ice
    if (mode==11) return pal(t, vec3(0.6,0.1,0.7), vec3(0.2,0.3,0.8), vec3(0.9,0.4,0.2), vec3(0.7,0.7,0.7)); // Galaxy
    if (mode==12) return pal(t, vec3(0.0,1.0,0.5), vec3(1.0,0.0,0.5), vec3(0.5,1.0,0.0), vec3(0.2,0.4,0.6)); // Aurora
    if (mode==13) return pal(t, vec3(0.0,0.4,0.0), vec3(0.4,0.8,0.4), vec3(0.2,0.6,0.2), vec3(0.2,0.4,0.6)); // Rainforest
    if (mode==14) return pal(t, vec3(0.9,0.8,0.5), vec3(0.8,0.7,0.2), vec3(0.6,0.4,0.2), vec3(0.2,0.4,0.6)); // Desert
    if (mode==15) return pal(t, vec3(0.9,0.7,0.3), vec3(0.4,0.2,0.1), vec3(0.8,0.6,0.4), vec3(0.1,0.2,0.3)); // Vintage
    if (mode==16) return pal(t, vec3(1.0,0.0,1.0), vec3(0.0,1.0,1.0), vec3(1.0,1.0,0.0), vec3(0.2,0.4,0.6)); // Electric
    if (mode==17) return pal(t, vec3(0.8,0.9,1.0), vec3(0.2,0.3,0.4), vec3(0.5,0.6,0.7), vec3(0.2,0.4,0.6)); // Frost
    if (mode==18) return pal(t, vec3(1.0,0.5,0.0), vec3(0.6,0.2,0.1), vec3(0.9,0.4,0.2), vec3(0.7,0.3,0.2)); // Sunrise
    if (mode==19) return pal(t, vec3(0.2,0.2,0.2), vec3(0.4,0.4,0.4), vec3(0.6,0.6,0.6), vec3(0.8,0.8,0.8)); // Dusk
    return pal(t, vec3(0.5), vec3(0.5), vec3(0.5), vec3(0.0));
}

// Shape functions
float shapeSphere(vec3 p) {
    return length(p) - 1.0;
}
float shapeTorus(vec3 p) {
    vec2 q= vec2(length(p.xz)-1.0,p.y);
    return length(q)-0.3;
}
float shapeMandelbox(vec3 p) {
    vec3 z= p;
    float scale= 2.0;
    float dr= 1.0;
    for (int i=0; i<10; i++) {
        z= clamp(z, -1.0, 1.0)*2.0 - z; // box fold
        float r2= dot(z,z);
        if (r2<0.25) {
            z*=4.0; dr*=4.0;
        } else if (r2<4.0) {
            float c= (scale-1.0)/r2;
            z*= c; dr*= c;
        }
        z= z*scale + p;
    }
    return length(z)-1.0;
}
float shapeSierpinski(vec3 p) {
    for (int i=0; i<5; i++) {
        p= abs(p);
        p= p*3.0 - vec3(1.5);
        if (p.x + p.y + p.z > 4.5) p= vec3(4.5)-p;
    }
    return length(p) - 0.2;
}
float shapeKleinBottle(vec3 p) {
    // Placeholder shape
    float u= p.x;
    float v= p.z;
    float x= (2.0/PI)*cos(u)*(1.0 + sin(u))*0.5;
    float y= (2.0/PI)*sin(u)*(1.0 + sin(u))*0.5;
    float z= v + (cos(u)*sin(u));
    return length(vec3(x,y,z)-p);
}
float shapeOctahedron(vec3 p) {
    p= abs(p);
    float m= p.x + p.y + p.z -1.0;
    return m;
}
float shapeChaos(vec3 p, float chaos) {
    float c= sin(p.x*3.0 + TIME*ChaosSpeed) + sin(p.y*4.0 + TIME*ChaosSpeed*1.2) + sin(p.z*5.0 + TIME*ChaosSpeed*0.8);
    return c * chaos;
}

// Main scene function
float scene(vec3 p, int geomType, float chaos, float chaosMix) {
    float baseDist= 0.0;
    if (geomType==0) baseDist= shapeSphere(p);
    else if (geomType==1) baseDist= shapeTorus(p);
    else if (geomType==2) baseDist= shapeMandelbox(p);
    else if (geomType==3) baseDist= shapeSierpinski(p);
    else if (geomType==4) baseDist= shapeChaos(p, chaos);
    else if (geomType==5) baseDist= shapeKleinBottle(p);
    else if (geomType==6) baseDist= shapeOctahedron(p);
    // Mix with chaos shape based on chaosMix
    float chaosDist= shapeChaos(p, chaos);
    return mix(baseDist, chaosDist, chaosMix);
}

// Transformation function with symmetry, chaos, chaosspeed
vec3 applyTransform(vec3 p, int mode, float chaos, float symmetry, float chaosspeed) {
    // Apply symmetry as a scale
    p= p * symmetry;
    if (mode==1) {
        // Symmetric
        p= vec3(abs(p.x), abs(p.y), abs(p.z));
    } else if (mode==2) {
        // Chaos distortion
        p+= sin(p*3.0+TIME*chaosspeed)*chaos*0.3;
    } else if (mode==3) {
        // MultiFractal
        p+= sin(p*(1.0+chaos*2.0)+TIME*chaosspeed)*chaos*0.5;
        p= fract(p*1.5)-0.75;
    } else if (mode==4) {
        // Swirl
        float angle= atan(p.z,p.x);
        float radius= length(vec2(p.x,p.z));
        angle+= TIME*chaosspeed*0.2;
        p.x= cos(angle)*radius;
        p.z= sin(angle)*radius;
    } else if (mode==5) {
        // Spiral
        float a= atan(p.z,p.x);
        float r= length(vec2(p.x,p.z));
        a+= TIME*chaosspeed*0.3;
        p.x= cos(a)*r;
        p.z= sin(a)*r;
    }
    return p;
}

// Main fragment shader
void main() {
    vec2 uv= (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    float t= TIME*Speed;

    vec3 origin= vec3(0.0,0.0,-3.0);
    vec3 dir= normalize(vec3(uv*Zoom,1.0));

    // Feedback buffer sampling
    vec4 feedbackColor = texture2D(BufferA, fract(gl_FragCoord.xy / RENDERSIZE.xy));
    vec3 feedback= feedbackColor.rgb;

    vec3 color= vec3(0.0);
    float dist= 0.0;

    int geomType= int(GeometryType);
    int mode= int(TransformMode);
    float chaos= ChaosIntensity;
    float chaosspeed= ChaosSpeed;
    float symmetry= Symmetry;
    float chaosMix= ChaosMix;
    float brightness= Brightness;
    float contrast= Contrast;
    float glow= Glow;
    float feedbackAmt= FeedbackAmount;

    for (int i=0; i<64; i++) {
        vec3 p= origin + dir*dist;
        p= applyTransform(p, mode, chaos, symmetry, chaosspeed);
        float d= scene(p, geomType, chaos, chaosMix);
        // add chaotic sine waves
        d+= sin(p.x*3.0 + t) + sin(p.y*4.0 + t*1.2) + sin(p.z*5.0 + t*0.8);
        dist+= max(abs(d),0.01);
        float fade= exp(-float(i)*0.05);
        vec3 col= getColorPalette( int(ColorPaletteMode), p.z + t*0.2);
        float brightnessVal= 0.005 / (0.01 + d*d);
        color+= brightnessVal * fade * col;
        if (dist>20.0) break;
    }

    // Psychedelic pulse
    float pulse= sin(TIME*ChaosSpeed)*0.5+0.5;
    color*= 1.0 + 0.3*pulse;

    // Apply contrast and brightness
    color= (color - 0.5) * contrast + 0.5;
    color*= brightness;

    // Mix feedback and glow
    color= mix(color, feedback, feedbackAmt);
    color*= glow;

    gl_FragColor= vec4(color,1.0);
}