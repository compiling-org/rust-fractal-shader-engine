odular-fractal-shader/src/export/mod.rs</path>
<content lines="1-150">
pub mod video;
pub mod image_sequence;

/// Video recording and export functionality
pub struct VideoExporter {
    frame_buffer: Vec<u8>,
    width: u32,
    height: u32,
    frame_rate: f32,
    current_frame: u32,
    recording: bool,
    output_path: String,
    codec_settings: VideoCodecSettings,
}

impl VideoExporter {
    pub fn new(width: u32, height: u32, frame_rate: f32, output_path: String) -> Self {
        Self {
            frame_buffer: vec![0; (width * height * 4) as usize],
            width,
            height,
            frame_rate,
            current_frame: 0,
            recording: false,
            output_path,
            codec_settings: VideoCodecSettings::default(),
        }
    }

    pub fn start_recording(&mut self) -> Result<(), ExportError> {
        if self.recording {
            return Err(ExportError::AlreadyRecording);
        }

        self.recording = true;
        self.current_frame = 0;

        // Initialize video encoder
        self.initialize_encoder()?;

        log::info!("Started video recording: {}x{} @ {}fps", self.width, self.height, self.frame_rate);
        Ok(())
    }

    pub fn stop_recording(&mut self) -> Result<String, ExportError> {
        if !self.recording {
            return Err(ExportError::NotRecording);
        }

        self.recording = false;

        // Finalize video file
        let output_file = self.finalize_video()?;

        log::info!("Video recording completed: {}", output_file);
        Ok(output_file)
    }

    pub fn add_frame(&mut self, render_result: &crate::rendering::RenderResult) -> Result<(), ExportError> {
        if !self.recording {
            return Ok(()); // Silently ignore if not recording
        }

        // Convert render result to RGBA frame buffer
        self.convert_render_result_to_framebuffer(render_result);

        // Encode frame
        self.encode_frame(&self.frame_buffer)?;

        self.current_frame += 1;
        Ok(())
    }

    pub fn is_recording(&self) -> bool {
        self.recording
    }

    pub fn current_frame(&self) -> u32 {
        self.current_frame
    }

    pub fn set_codec_settings(&mut self, settings: VideoCodecSettings) {
        self.codec_settings = settings;
    }

    fn initialize_encoder(&self) -> Result<(), ExportError> {
        // TODO: Initialize video encoder (FFmpeg, libx264, etc.)
        log::info!("Initializing video encoder with codec: {:?}", self.codec_settings.codec);
        Ok(())
    }

    fn finalize_video(&self) -> Result<String, ExportError> {
        // TODO: Finalize and save video file
        let output_file = format!("{}/fractal_animation_{}.{}", self.output_path, chrono::Utc::now().timestamp(), self.codec_settings.extension());
        log::info!("Finalizing video file: {}", output_file);
        Ok(output_file)
    }

    fn convert_render_result_to_framebuffer(&mut self, render_result: &crate::rendering::RenderResult) {
        // Convert float color buffer to RGBA bytes
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

    fn encode_frame(&self, frame_buffer: &[u8]) -> Result<(), ExportError> {
        // TODO: Encode frame using video codec
        log::debug!("Encoding frame {}", self.current_frame);
        Ok(())
    }
}

/// Image sequence exporter
pub struct ImageSequenceExporter {
    output_path: String,
    frame_number: u32,
    format: ImageFormat,
    quality: u8,
}

impl ImageSequenceExporter {
    pub fn new(output_path: String, format: ImageFormat) -> Self {
        Self {
            output_path,
            frame_number: 0,
            format,
            quality: 95, // Default quality
        }
    }

    pub fn export_frame(&mut self, render_result: &crate::rendering::RenderResult) -> Result<String, ExportError> {
        let filename = format!(
            "{}/frame_{:06}.{}",
            self.output_path,
            self.frame_number,
            self.format.extension()
        );

        // Convert render result to image buffer
        let image_buffer = self.convert_to_image_buffer(render_result)?;

        // Save image
        self.save_image(&image_buffer, &filename)?;

        self.frame_number += 1;
        Ok(filename)
    }

    pub fn set_quality(&mut self, quality: u8) {
        self.quality = quality.clamp(1, 100);
    }

    pub fn reset_frame_counter(&mut self) {
        self.frame_number = 0;
    }

    fn convert_to_image_buffer(&self, render_result: &crate::rendering::RenderResult) -> Result<Vec<u8>, ExportError> {
        let width = (render_result.color_buffer.len() / 3) as u32;
        let height = 1; // Assume single row for now, would need to be calculated properly

        let mut image_buffer = Vec::with_capacity(width as usize * height as usize * 4);

        for chunk in render_result.color_buffer.chunks(3) {
            if chunk.len() == 3 {
                let r = (chunk[0].clamp(0.0, 1.0) * 255.0) as u8;
                let g = (chunk[1].clamp(0.0, 1.0) * 255.0) as u8;
                let b = (chunk[2].clamp(0.0, 1.0) * 255.0) as u8;
                let a = 255u8;

                image_buffer.extend_from_slice(&[r, g, b, a]);
            }
        }

        Ok(image_buffer)
    }

    fn save_image(&self, image_buffer: &[u8], filename: &str) -> Result<(), ExportError> {
        // TODO: Save image using image crate or similar
        log::info!("Saving image: {} ({} bytes)", filename, image_buffer.len());
        Ok(())
    }
}

/// Video codec settings
#[derive(Debug, Clone)]
pub struct VideoCodecSettings {
    pub codec: VideoCodec,
    pub bitrate: u32, // kbps
    pub preset: String,
    pub profile: String,
}

impl VideoCodecSettings {
    pub fn extension(&self) -> &'static str {
        match self.codec {
            VideoCodec::H264 => "mp4",
            VideoCodec::H265 => "mp4",
            VideoCodec::VP9 => "webm",
            VideoCodec::AV1 => "mkv",
        }
    }
}

impl Default for VideoCodecSettings {
    fn default() -> Self {
        Self {
            codec: VideoCodec::H264,
            bitrate: 5000, // 5 Mbps
            preset: "medium".to_string(),
            profile: "high".to_string(),
        }
    }
}

/// Supported video codecs
#[derive(Debug, Clone)]
pub enum VideoCodec {
    H264,
    H265,
    VP9,
    AV1,
}

/// Supported image formats
#[derive(Debug, Clone)]
pub enum ImageFormat {
    PNG,
    JPEG,
    TIFF,
    EXR,
}

impl ImageFormat {
    pub fn extension(&self) -> &'static str {
        match self {
            ImageFormat::PNG => "png",
            ImageFormat::JPEG => "jpg",
            ImageFormat::TIFF => "tiff",
            ImageFormat::EXR => "exr",
        }
    }
}

/// Export-related errors
#[derive(Debug, Clone)]
pub enum ExportError {
    AlreadyRecording,
    NotRecording,
    EncoderInitializationFailed(String),
    EncodingFailed(String),
    FileWriteError(String),
    InvalidFrameData(String),
}

impl std::fmt::Display for ExportError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            ExportError::AlreadyRecording => write!(f, "Already recording"),
            ExportError::NotRecording => write!(f, "Not currently recording"),
            ExportError::EncoderInitializationFailed(msg) => write!(f, "Encoder initialization failed: {}", msg),
            ExportError::EncodingFailed(msg) => write!(f, "Encoding failed: {}", msg),
            ExportError::FileWriteError(msg) => write!(f, "File write error: {}", msg),
            ExportError::InvalidFrameData(msg) => write!(f, "Invalid frame data: {}", msg),
        }
    }
}

impl std::error::Error for ExportError {}

/// Export manager for coordinating different export types
pub struct ExportManager {
    video_exporter: Option<VideoExporter>,
    image_exporter: Option<ImageSequenceExporter>,
    export_queue: Vec<ExportJob>,
}

impl ExportManager {
    pub fn new() -> Self {
        Self {
            video_exporter: None,
            image_exporter: None,
            export_queue: Vec::new(),
        }
    }

    pub fn start_video_export(&mut self, settings: VideoExportSettings) -> Result<(), ExportError> {
        let exporter = VideoExporter::new(
            settings.width,
            settings.height,
            settings.frame_rate,
            settings.output_path,
        );

        self.video_exporter = Some(exporter);
        if let Some(ref mut exporter) = self.video_exporter {
            exporter.start_recording()?;
        }

        Ok(())
    }

    pub fn start_image_sequence_export(&mut self, settings: ImageSequenceExportSettings) -> Result<(), ExportError> {
        let exporter = ImageSequenceExporter::new(
            settings.output_path,
            settings.format,
        );

        self.image_exporter = Some(exporter);
        Ok(())
    }

    pub fn process_frame(&mut self, render_result: &crate::rendering::RenderResult) -> Result<(), ExportError> {
        // Export to video if recording
        if let Some(ref mut exporter) = self.video_exporter {
            if exporter.is_recording() {
                exporter.add_frame(render_result)?;
            }
        }

        // Export to image sequence if active
        if let Some(ref mut exporter) = self.image_exporter {
            exporter.export_frame(render_result)?;
        }

        Ok(())
    }

    pub fn stop_exports(&mut self) -> Result<ExportResults, ExportError> {
        let mut results = ExportResults::default();

        // Stop video export
        if let Some(exporter) = self.video_exporter.take() {
            if exporter.is_recording() {
                results.video_file = Some(exporter.stop_recording()?);
            }
        }

        // Image sequence export is complete when frames are done
        results.image_sequence_count = self.image_exporter.as_ref().map(|e| e.frame_number).unwrap_or(0);

        Ok(results)
    }
}

/// Export settings
#[derive(Debug, Clone)]
pub struct VideoExportSettings {
    pub width: u32,
    pub height: u32,
    pub frame_rate: f32,
    pub output_path: String,
    pub codec_settings: VideoCodecSettings,
}

#[derive(Debug, Clone)]
pub struct ImageSequenceExportSettings {
    pub output_path: String,
    pub format: ImageFormat,
    pub quality: u8,
}

/// Export job for queued operations
#[derive(Debug, Clone)]
pub struct ExportJob {
    pub job_type: ExportJobType,
    pub settings: ExportSettings,
    pub priority: u8,
}

#[derive(Debug, Clone)]
pub enum ExportJobType {
    Video,
    ImageSequence,
    Mesh,
    Voxel,
}

#[derive(Debug, Clone)]
pub enum ExportSettings {
    Video(VideoExportSettings),
    ImageSequence(ImageSequenceExportSettings),
}

/// Export results
#[derive(Debug, Default)]
pub struct ExportResults {
    pub video_file: Option<String>,
    pub image_sequence_count: u32,
    pub mesh_file: Option<String>,
    pub voxel_file: Option<String>,
}