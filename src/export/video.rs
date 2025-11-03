//! Video Export Module
//!
//! This module handles exporting animations and fractal visualizations
//! as video files using various codecs and formats.

use super::*;
use std::process::Command;

/// Video export settings
#[derive(Debug, Clone)]
pub struct VideoSettings {
    pub width: u32,
    pub height: u32,
    pub frame_rate: u32,
    pub duration_seconds: f32,
    pub codec: VideoCodec,
    pub quality: VideoQuality,
    pub audio_enabled: bool,
    pub audio_path: Option<std::path::PathBuf>,
}

/// Supported video codecs
#[derive(Debug, Clone, Copy)]
pub enum VideoCodec {
    H264,
    H265,
    VP9,
    AV1,
    ProRes,
}

/// Video quality presets
#[derive(Debug, Clone, Copy)]
pub enum VideoQuality {
    Low,      // Fast encoding, smaller file
    Medium,   // Balanced quality/size
    High,     // High quality, larger file
    Ultra,    // Maximum quality
}

impl VideoQuality {
    /// Get ffmpeg quality parameters
    pub fn ffmpeg_params(&self) -> (&str, &str) {
        match self {
            VideoQuality::Low => ("-preset fast", "-crf 28"),
            VideoQuality::Medium => ("-preset medium", "-crf 23"),
            VideoQuality::High => ("-preset slow", "-crf 18"),
            VideoQuality::Ultra => ("-preset veryslow", "-crf 16"),
        }
    }
}

/// Video exporter using ffmpeg
pub struct VideoExporter {
    settings: VideoSettings,
    temp_dir: std::path::PathBuf,
    frame_count: u32,
}

impl VideoExporter {
    /// Create new video exporter
    pub fn new(settings: VideoSettings) -> Result<Self, ExportError> {
        let temp_dir = std::env::temp_dir().join("fractal_video_export");
        std::fs::create_dir_all(&temp_dir)?;

        Ok(Self {
            settings,
            temp_dir,
            frame_count: 0,
        })
    }

    /// Add frame to video
    pub fn add_frame(&mut self, pixels: &[u8]) -> Result<(), ExportError> {
        let frame_filename = format!("frame_{:08}.png", self.frame_count);
        let frame_path = self.temp_dir.join(frame_filename);

        // Export frame as PNG
        ImageExporter::export_image(
            pixels,
            self.settings.width,
            self.settings.height,
            ExportFormat::PNG,
            &frame_path,
        )?;

        self.frame_count += 1;
        Ok(())
    }

    /// Finalize video export
    pub fn finalize(&self, output_path: &std::path::Path) -> Result<(), ExportError> {
        let input_pattern = self.temp_dir.join("frame_%08d.png");

        // Build ffmpeg command
        let mut cmd = Command::new("ffmpeg");

        // Input options
        cmd.arg("-framerate").arg(self.settings.frame_rate.to_string());
        cmd.arg("-i").arg(input_pattern);

        // Video codec options
        match self.settings.codec {
            VideoCodec::H264 => {
                cmd.arg("-c:v").arg("libx264");
                let (preset, crf) = self.settings.quality.ffmpeg_params();
                cmd.arg(preset.split(' ').get(1).unwrap())
                   .arg(preset.split(' ').get(0).unwrap().trim_start_matches('-'));
                cmd.arg(crf.split(' ').get(1).unwrap())
                   .arg(crf.split(' ').get(0).unwrap().trim_start_matches('-'));
                cmd.arg("-pix_fmt").arg("yuv420p");
            }
            VideoCodec::H265 => {
                cmd.arg("-c:v").arg("libx265");
                let (preset, crf) = self.settings.quality.ffmpeg_params();
                cmd.arg(preset.split(' ').get(1).unwrap())
                   .arg(preset.split(' ').get(0).unwrap().trim_start_matches('-'));
                cmd.arg("-crf").arg("23");
                cmd.arg("-pix_fmt").arg("yuv420p10le");
            }
            VideoCodec::VP9 => {
                cmd.arg("-c:v").arg("libvpx-vp9");
                cmd.arg("-crf").arg("30");
                cmd.arg("-b:v").arg("0");
            }
            VideoCodec::AV1 => {
                cmd.arg("-c:v").arg("libaom-av1");
                cmd.arg("-crf").arg("30");
                cmd.arg("-b:v").arg("0");
                cmd.arg("-cpu-used").arg("8");
            }
            VideoCodec::ProRes => {
                cmd.arg("-c:v").arg("prores_ks");
                cmd.arg("-profile:v").arg("3");
                cmd.arg("-pix_fmt").arg("yuv422p10le");
            }
        }

        // Audio options
        if self.settings.audio_enabled {
            if let Some(audio_path) = &self.settings.audio_path {
                cmd.arg("-i").arg(audio_path);
                cmd.arg("-c:a").arg("aac");
                cmd.arg("-b:a").arg("192k");
                cmd.arg("-shortest");
            }
        } else {
            cmd.arg("-an"); // No audio
        }

        // Output file
        cmd.arg(output_path);

        // Execute ffmpeg
        let output = cmd.output()?;

        if !output.status.success() {
            let stderr = String::from_utf8_lossy(&output.stderr);
            return Err(ExportError::IoError(std::io::Error::new(
                std::io::ErrorKind::Other,
                format!("ffmpeg failed: {}", stderr),
            )));
        }

        // Clean up temporary files
        self.cleanup()?;

        Ok(())
    }

    /// Clean up temporary files
    fn cleanup(&self) -> Result<(), ExportError> {
        for entry in std::fs::read_dir(&self.temp_dir)? {
            let entry = entry?;
            std::fs::remove_file(entry.path())?;
        }
        std::fs::remove_dir(&self.temp_dir)?;
        Ok(())
    }

    /// Get export progress (0.0 to 1.0)
    pub fn progress(&self) -> f32 {
        let total_frames = (self.settings.duration_seconds * self.settings.frame_rate as f32) as u32;
        if total_frames == 0 {
            return 1.0;
        }
        (self.frame_count as f32 / total_frames as f32).min(1.0)
    }

    /// Check if export is complete
    pub fn is_complete(&self) -> bool {
        let expected_frames = (self.settings.duration_seconds * self.settings.frame_rate as f32) as u32;
        self.frame_count >= expected_frames
    }
}

/// Real-time video streaming
pub struct VideoStreamer {
    settings: VideoSettings,
    streaming_cmd: Option<std::process::Child>,
}

impl VideoStreamer {
    /// Start streaming to RTMP endpoint
    pub fn start_streaming(settings: VideoSettings, rtmp_url: &str) -> Result<Self, ExportError> {
        let mut cmd = Command::new("ffmpeg");

        // Input from pipe
        cmd.arg("-f").arg("rawvideo");
        cmd.arg("-pixel_format").arg("rgba");
        cmd.arg("-video_size").arg(format!("{}x{}", settings.width, settings.height));
        cmd.arg("-framerate").arg(settings.frame_rate.to_string());
        cmd.arg("-i").arg("-");

        // Output to RTMP
        cmd.arg("-c:v").arg("libx264");
        cmd.arg("-preset").arg("veryfast");
        cmd.arg("-maxrate").arg("3000k");
        cmd.arg("-bufsize").arg("6000k");
        cmd.arg("-pix_fmt").arg("yuv420p");
        cmd.arg("-g").arg("50");
        cmd.arg("-c:a").arg("aac");
        cmd.arg("-b:a").arg("128k");
        cmd.arg("-ac").arg("2");
        cmd.arg("-ar").arg("44100");
        cmd.arg("-f").arg("flv");
        cmd.arg(rtmp_url);

        let streaming_cmd = Some(cmd.spawn()?);

        Ok(Self {
            settings,
            streaming_cmd,
        })
    }

    /// Send frame to stream
    pub fn send_frame(&mut self, pixels: &[u8]) -> Result<(), ExportError> {
        if let Some(ref mut cmd) = self.streaming_cmd {
            use std::io::Write;
            if let Some(ref mut stdin) = cmd.stdin {
                stdin.write_all(pixels)?;
                stdin.flush()?;
            }
        }
        Ok(())
    }

    /// Stop streaming
    pub fn stop_streaming(&mut self) -> Result<(), ExportError> {
        if let Some(mut cmd) = self.streaming_cmd.take() {
            cmd.kill()?;
            cmd.wait()?;
        }
        Ok(())
    }
}

/// Video format detection and validation
pub struct VideoFormatValidator;

impl VideoFormatValidator {
    /// Check if ffmpeg is available
    pub fn check_ffmpeg() -> bool {
        Command::new("ffmpeg")
            .arg("-version")
            .output()
            .map(|output| output.status.success())
            .unwrap_or(false)
    }

    /// Get supported codecs
    pub fn get_supported_codecs() -> Vec<VideoCodec> {
        let mut supported = vec![VideoCodec::H264]; // H264 is most widely supported

        if Self::check_codec_support("libx265") {
            supported.push(VideoCodec::H265);
        }
        if Self::check_codec_support("libvpx-vp9") {
            supported.push(VideoCodec::VP9);
        }
        if Self::check_codec_support("libaom-av1") {
            supported.push(VideoCodec::AV1);
        }
        if Self::check_codec_support("prores_ks") {
            supported.push(VideoCodec::ProRes);
        }

        supported
    }

    /// Check if specific codec is supported
    fn check_codec_support(codec: &str) -> bool {
        Command::new("ffmpeg")
            .arg("-codecs")
            .output()
            .map(|output| {
                let stdout = String::from_utf8_lossy(&output.stdout);
                stdout.contains(codec)
            })
            .unwrap_or(false)
    }

    /// Validate video settings
    pub fn validate_settings(settings: &VideoSettings) -> Result<(), String> {
        if settings.width == 0 || settings.height == 0 {
            return Err("Invalid video dimensions".to_string());
        }

        if settings.frame_rate == 0 {
            return Err("Invalid frame rate".to_string());
        }

        if settings.duration_seconds <= 0.0 {
            return Err("Invalid duration".to_string());
        }

        if settings.audio_enabled && settings.audio_path.is_none() {
            return Err("Audio enabled but no audio file specified".to_string());
        }

        Ok(())
    }
}

/// Export presets for common video formats
pub struct VideoPresets;

impl VideoPresets {
    /// Create settings for YouTube 1080p
    pub fn youtube_1080p(duration: f32) -> VideoSettings {
        VideoSettings {
            width: 1920,
            height: 1080,
            frame_rate: 30,
            duration_seconds: duration,
            codec: VideoCodec::H264,
            quality: VideoQuality::High,
            audio_enabled: true,
            audio_path: None,
        }
    }

    /// Create settings for Vimeo 4K
    pub fn vimeo_4k(duration: f32) -> VideoSettings {
        VideoSettings {
            width: 3840,
            height: 2160,
            frame_rate: 24,
            duration_seconds: duration,
            codec: VideoCodec::H265,
            quality: VideoQuality::Ultra,
            audio_enabled: true,
            audio_path: None,
        }
    }

    /// Create settings for Instagram Reels
    pub fn instagram_reels(duration: f32) -> VideoSettings {
        VideoSettings {
            width: 1080,
            height: 1920, // Vertical video
            frame_rate: 30,
            duration_seconds: duration,
            codec: VideoCodec::H264,
            quality: VideoQuality::High,
            audio_enabled: true,
            audio_path: None,
        }
    }

    /// Create settings for archival quality
    pub fn archival_prores(duration: f32) -> VideoSettings {
        VideoSettings {
            width: 1920,
            height: 1080,
            frame_rate: 24,
            duration_seconds: duration,
            codec: VideoCodec::ProRes,
            quality: VideoQuality::Ultra,
            audio_enabled: false,
            audio_path: None,
        }
    }
}