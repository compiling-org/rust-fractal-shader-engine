odular-fractal-shader/src/output/mod.rs</path>
<content lines="1-150">
pub mod video_mapping;
pub mod installations;

/// Real-time output system for video mapping and installations
pub struct OutputSystem {
    video_outputs: Vec<VideoOutput>,
    installation_configs: Vec<InstallationConfig>,
    frame_buffer: Vec<u8>,
    width: u32,
    height: u32,
}

impl OutputSystem {
    pub fn new(width: u32, height: u32) -> Self {
        Self {
            video_outputs: Vec::new(),
            installation_configs: Vec::new(),
            frame_buffer: vec![0; (width * height * 4) as usize], // RGBA
            width,
            height,
        }
    }

    pub fn add_video_output(&mut self, output: VideoOutput) {
        self.video_outputs.push(output);
    }

    pub fn add_installation_config(&mut self, config: InstallationConfig) {
        self.installation_configs.push(config);
    }

    pub fn update_frame_buffer(&mut self, render_result: &crate::rendering::RenderResult) {
        // Convert render result to RGBA frame buffer
        for (i, &value) in render_result.color_buffer.iter().enumerate() {
            let pixel_index = i / 3;
            let channel = i % 3;

            if pixel_index < self.frame_buffer.len() / 4 {
                let byte_value = (value.clamp(0.0, 1.0) * 255.0) as u8;
                self.frame_buffer[pixel_index * 4 + channel] = byte_value;
                if channel == 2 { // Set alpha to 255
                    self.frame_buffer[pixel_index * 4 + 3] = 255;
                }
            }
        }
    }

    pub fn render_to_outputs(&self) {
        // Send frame buffer to all video outputs
        for output in &self.video_outputs {
            output.send_frame(&self.frame_buffer);
        }

        // Apply installation-specific transformations
        for config in &self.installation_configs {
            config.apply_transformations(&self.frame_buffer);
        }
    }

    pub fn get_frame_buffer(&self) -> &[u8] {
        &self.frame_buffer
    }
}

/// Video output destination
pub struct VideoOutput {
    name: String,
    output_type: VideoOutputType,
    resolution: (u32, u32),
    frame_rate: f32,
}

impl VideoOutput {
    pub fn new(name: String, output_type: VideoOutputType, resolution: (u32, u32), frame_rate: f32) -> Self {
        Self {
            name,
            output_type,
            resolution,
            frame_rate,
        }
    }

    pub fn send_frame(&self, frame_buffer: &[u8]) {
        match &self.output_type {
            VideoOutputType::Syphon => {
                // Send to Syphon (macOS)
                self.send_to_syphon(frame_buffer);
            }
            VideoOutputType::Spout => {
                // Send to Spout (Windows)
                self.send_to_spout(frame_buffer);
            }
            VideoOutputType::NDI => {
                // Send via NDI
                self.send_via_ndi(frame_buffer);
            }
            VideoOutputType::DMX => {
                // Convert to DMX lighting data
                self.convert_to_dmx(frame_buffer);
            }
            VideoOutputType::ProjectionMapping => {
                // Apply projection mapping transformations
                self.apply_projection_mapping(frame_buffer);
            }
        }
    }

    fn send_to_syphon(&self, _frame_buffer: &[u8]) {
        // TODO: Implement Syphon output
        println!("Sending frame to Syphon output: {}", self.name);
    }

    fn send_to_spout(&self, _frame_buffer: &[u8]) {
        // TODO: Implement Spout output
        println!("Sending frame to Spout output: {}", self.name);
    }

    fn send_via_ndi(&self, _frame_buffer: &[u8]) {
        // TODO: Implement NDI output
        println!("Sending frame via NDI: {}", self.name);
    }

    fn convert_to_dmx(&self, frame_buffer: &[u8]) {
        // Convert frame buffer to DMX lighting data
        // Sample pixels at specific positions for lighting fixtures
        let dmx_data = self.sample_pixels_for_lighting(frame_buffer);
        self.send_dmx_data(&dmx_data);
    }

    fn apply_projection_mapping(&self, frame_buffer: &[u8]) {
        // Apply geometric corrections for projection mapping
        let corrected_frame = self.apply_keystone_correction(frame_buffer);
        let blended_frame = self.apply_edge_blending(&corrected_frame);
        self.output_corrected_frame(&blended_frame);
    }

    fn sample_pixels_for_lighting(&self, frame_buffer: &[u8]) -> Vec<u8> {
        let mut dmx_data = Vec::new();

        // Sample pixels at predefined positions for lighting fixtures
        let sample_positions = [
            (100, 100), (200, 200), (300, 100), (400, 200), // Example positions
        ];

        for (x, y) in sample_positions {
            if (x < self.resolution.0) && (y < self.resolution.1) {
                let pixel_index = ((y * self.resolution.0 + x) * 4) as usize;
                if pixel_index + 2 < frame_buffer.len() {
                    // Convert RGB to intensity and hue for lighting
                    let r = frame_buffer[pixel_index] as f32 / 255.0;
                    let g = frame_buffer[pixel_index + 1] as f32 / 255.0;
                    let b = frame_buffer[pixel_index + 2] as f32 / 255.0;

                    let intensity = (r + g + b) / 3.0;
                    let hue = self.rgb_to_hue(r, g, b);

                    dmx_data.push((intensity * 255.0) as u8);
                    dmx_data.push((hue * 255.0) as u8);
                    dmx_data.push(0); // Saturation placeholder
                }
            }
        }

        dmx_data
    }

    fn rgb_to_hue(&self, r: f32, g: f32, b: f32) -> f32 {
        let max = r.max(g).max(b);
        let min = r.min(g).min(b);
        let delta = max - min;

        if delta == 0.0 {
            return 0.0;
        }

        let hue = if max == r {
            ((g - b) / delta) % 6.0
        } else if max == g {
            (b - r) / delta + 2.0
        } else {
            (r - g) / delta + 4.0
        };

        (hue / 6.0).clamp(0.0, 1.0)
    }

    fn send_dmx_data(&self, _dmx_data: &[u8]) {
        // TODO: Send DMX data to lighting controller
        println!("Sending DMX data for lighting fixtures");
    }

    fn apply_keystone_correction(&self, frame_buffer: &[u8]) -> Vec<u8> {
        // TODO: Implement keystone correction
        frame_buffer.to_vec()
    }

    fn apply_edge_blending(&self, frame_buffer: &[u8]) -> Vec<u8> {
        // TODO: Implement edge blending
        frame_buffer.to_vec()
    }

    fn output_corrected_frame(&self, _frame_buffer: &[u8]) {
        // TODO: Output corrected frame
        println!("Outputting projection-mapped frame");
    }
}

/// Installation configuration for multi-screen setups
pub struct InstallationConfig {
    name: String,
    screens: Vec<ScreenConfig>,
    transformations: Vec<GeometricTransformation>,
}

impl InstallationConfig {
    pub fn new(name: String) -> Self {
        Self {
            name,
            screens: Vec::new(),
            transformations: Vec::new(),
        }
    }

    pub fn add_screen(&mut self, screen: ScreenConfig) {
        self.screens.push(screen);
    }

    pub fn add_transformation(&mut self, transformation: GeometricTransformation) {
        self.transformations.push(transformation);
    }

    pub fn apply_transformations(&self, frame_buffer: &[u8]) {
        // Apply all geometric transformations for this installation
        let mut transformed_buffer = frame_buffer.to_vec();

        for transformation in &self.transformations {
            transformed_buffer = transformation.apply(&transformed_buffer);
        }

        // Send to all screens
        for screen in &self.screens {
            screen.display_frame(&transformed_buffer);
        }
    }
}

/// Screen configuration for multi-display installations
pub struct ScreenConfig {
    id: u32,
    position: (i32, i32),
    size: (u32, u32),
    rotation: f32,
    brightness: f32,
}

impl ScreenConfig {
    pub fn new(id: u32, position: (i32, i32), size: (u32, u32)) -> Self {
        Self {
            id,
            position,
            size,
            rotation: 0.0,
            brightness: 1.0,
        }
    }

    pub fn display_frame(&self, frame_buffer: &[u8]) {
        // TODO: Display frame on specific screen
        println!("Displaying frame on screen {} at position {:?}", self.id, self.position);
    }
}

/// Geometric transformation for installations
pub struct GeometricTransformation {
    transformation_type: TransformationType,
    parameters: Vec<f32>,
}

impl GeometricTransformation {
    pub fn new(transformation_type: TransformationType, parameters: Vec<f32>) -> Self {
        Self {
            transformation_type,
            parameters,
        }
    }

    pub fn apply(&self, frame_buffer: &[u8]) -> Vec<u8> {
        match self.transformation_type {
            TransformationType::Warp => self.apply_warp(frame_buffer),
            TransformationType::Blend => self.apply_blend(frame_buffer),
            TransformationType::ColorCorrection => self.apply_color_correction(frame_buffer),
        }
    }

    fn apply_warp(&self, frame_buffer: &[u8]) -> Vec<u8> {
        // TODO: Implement geometric warping
        frame_buffer.to_vec()
    }

    fn apply_blend(&self, frame_buffer: &[u8]) -> Vec<u8> {
        // TODO: Implement blending
        frame_buffer.to_vec()
    }

    fn apply_color_correction(&self, frame_buffer: &[u8]) -> Vec<u8> {
        // TODO: Implement color correction
        frame_buffer.to_vec()
    }
}

/// Types of video outputs
#[derive(Debug, Clone)]
pub enum VideoOutputType {
    Syphon,           // macOS
    Spout,            // Windows
    NDI,              // Network Device Interface
    DMX,              // Lighting control
    ProjectionMapping,// Geometric corrections
}

/// Types of geometric transformations
#[derive(Debug, Clone)]
pub enum TransformationType {
    Warp,
    Blend,
    ColorCorrection,
}