//! Animation Export Module
//!
//! This module provides capabilities to export fractal animations and keyframe sequences
//! for use in video editing software and other 3D applications.

use crate::animation::timeline::*;
use crate::fractal::types::FractalParameters;
use nalgebra::Vector3;
use std::collections::HashMap;

/// Animation export settings
#[derive(Debug, Clone)]
pub struct AnimationExportSettings {
    pub format: AnimationFormat,
    pub frame_rate: f32,
    pub resolution: [u32; 2], // Width and height
    pub duration: f32, // Duration in seconds
    pub export_frames: ExportFrames,
    pub quality: AnimationQuality,
    pub compression: AnimationCompression,
    pub include_audio: bool,
}

/// Supported animation formats
#[derive(Debug, Clone)]
pub enum AnimationFormat {
    MP4,
    AVI,
    MOV,
    GIF,
    WEBM,
    PNG_Sequence,
    JPEG_Sequence,
    TIFF_Sequence,
    JSON_Keyframes,
    FBX_Animation,
    GLTF_Animation,
}

/// Frame export options
#[derive(Debug, Clone)]
pub enum ExportFrames {
    AllFrames,
    Keyframes,
    CustomInterval(u32),
}

/// Animation quality settings
#[derive(Debug, Clone)]
pub enum AnimationQuality {
    Preview, // Lower quality for fast preview
    Standard, // Standard quality
    High, // High quality
    Ultra, // Ultra quality (slow)
}

/// Animation compression options
#[derive(Debug, Clone)]
pub enum AnimationCompression {
    None,
    H264,
    H265,
    VP9,
    LZW,
    RLE,
}

/// Animation export result
#[derive(Debug, Clone)]
pub struct AnimationExportResult {
    pub frames_generated: u32,
    pub total_duration: f32,
    pub output_files: Vec<String>,
    pub metadata: AnimationMetadata,
}

#[derive(Debug, Clone)]
pub struct AnimationMetadata {
    pub frame_count: u32,
    pub average_frame_time: f32,
    pub total_render_time: f32,
    pub fractal_updates: u32,
    pub file_size_mb: f32,
}

/// Animation exporter for creating animated fractal sequences
pub struct AnimationExporter {
    settings: AnimationExportSettings,
}

impl AnimationExporter {
    pub fn new(settings: AnimationExportSettings) -> Self {
        Self { settings }
    }

    /// Export animation from timeline project
    pub fn export_animation(&self, project: &TimelineProject, output_path: &str) -> Result<AnimationExportResult, Box<dyn std::error::Error>> {
        let frame_rate = self.settings.frame_rate;
        let total_frames = (self.settings.duration * frame_rate) as u32;
        
        let mut frames_generated = 0;
        let mut output_files = Vec::new();

        // Generate frames based on export settings
        match &self.settings.export_frames {
            ExportFrames::AllFrames => {
                for frame in 0..total_frames {
                    let time = frame as f32 / frame_rate;
                    self.render_frame(project, frame, time, output_path)?;
                    frames_generated += 1;
                    
                    // Update progress
                    if frame % 10 == 0 {
                        println!("Rendered frame {}/{} ({:.1}%)", frame, total_frames, (frame as f32 / total_frames as f32) * 100.0);
                    }
                }
            }
            ExportFrames::Keyframes => {
                // Export only at keyframe times
                let keyframes = self.extract_keyframes(project);
                for (frame_num, time) in keyframes {
                    if frame_num < total_frames {
                        self.render_frame(project, frame_num, time, output_path)?;
                        frames_generated += 1;
                    }
                }
            }
            ExportFrames::CustomInterval(interval) => {
                for frame in (0..total_frames).step_by(*interval as usize) {
                    let time = frame as f32 / frame_rate;
                    self.render_frame(project, frame, time, output_path)?;
                    frames_generated += 1;
                }
            }
        }

        // Create final output based on format
        match self.settings.format {
            AnimationFormat::MP4 | AnimationFormat::AVI | AnimationFormat::MOV | AnimationFormat::WEBM => {
                output_files = self.combine_frames_to_video(output_path)?;
            }
            AnimationFormat::PNG_Sequence | AnimationFormat::JPEG_Sequence | AnimationFormat::TIFF_Sequence => {
                // Files are already created as individual frames
            }
            AnimationFormat::JSON_Keyframes => {
                self.export_keyframes_json(project, output_path)?;
            }
            AnimationFormat::FBX_Animation => {
                self.export_fbx_animation(project, output_path)?;
            }
            AnimationFormat::GLTF_Animation => {
                self.export_gltf_animation(project, output_path)?;
            }
            AnimationFormat::GIF => {
                output_files = self.create_gif_from_frames(output_path)?;
            }
        }

        let metadata = AnimationMetadata {
            frame_count: frames_generated,
            average_frame_time: self.calculate_average_frame_time(frames_generated),
            total_render_time: self.calculate_total_render_time(frames_generated),
            fractal_updates: frames_generated,
            file_size_mb: self.calculate_total_file_size(&output_files),
        };

        Ok(AnimationExportResult {
            frames_generated,
            total_duration: self.settings.duration,
            output_files,
            metadata,
        })
    }

    /// Render a single frame
    fn render_frame(&self, project: &TimelineProject, frame: u32, time: f32, output_path: &str) -> Result<String, Box<dyn std::error::Error>> {
        // Update project to the current time
        let mut project_clone = project.clone();
        project_clone.timeline_mut().set_current_time(time);
        project_clone.animation_controller_mut().set_time(time);

        // Get fractal parameters at this time
        let fractal_params = project_clone.animation_controller().get_fractal_value("main_fractal");

        // Render the fractal (simplified)
        let filename = format!("{}/frame_{:06}.png", output_path, frame);
        self.render_fractal_frame(&fractal_params, &filename, frame)?;

        Ok(filename)
    }

    /// Render a single fractal frame
    fn render_fractal_frame(&self, params: &Option<FractalParameters>, filename: &str, frame: u32) -> Result<(), Box<dyn std::error::Error>> {
        let fractal_params = params.as_ref().unwrap_or(&FractalParameters::default());
        let (width, height) = (self.settings.resolution[0], self.settings.resolution[1]);

        // Create image buffer
        let mut image = image::RgbImage::new(width, height);

        // Render fractal (simplified implementation)
        for y in 0..height {
            for x in 0..width {
                let normalized_x = (x as f32 / width as f32) * 2.0 - 1.0;
                let normalized_y = (y as f32 / height as f32) * 2.0 - 1.0;

                // Calculate fractal distance (simplified)
                let point = Vector3::new(normalized_x, normalized_y, 0.0);
                let distance = self.calculate_fractal_distance(&fractal_params.formula, point);

                // Convert distance to color
                let color = self.distance_to_color(distance, frame);
                image.put_pixel(x, y, image::Rgb(color));
            }
        }

        // Save image
        image.save(filename)?;
        Ok(())
    }

    /// Calculate distance for fractal formula
    fn calculate_fractal_distance(&self, formula: &crate::fractal::types::FractalFormula, point: Vector3<f32>) -> f32 {
        // This would use the actual fractal engine
        // For now, simplified implementation
        match formula {
            crate::fractal::types::FractalFormula::Mandelbulb { power, .. } => {
                let r = point.magnitude();
                if r == 0.0 { return 0.0; }
                
                let theta = (point.z / r).acos();
                let phi = point.y.atan2(point.x);
                
                let new_r = r.powf(*power);
                let new_theta = theta * power;
                let new_phi = phi * power;
                
                let z_new = Vector3::new(
                    new_r * new_theta.sin() * new_phi.cos(),
                    new_r * new_theta.sin() * new_phi.sin(),
                    new_r * new_theta.cos(),
                );
                
                (z_new - point).magnitude()
            }
            _ => point.magnitude() - 1.0, // Simple sphere as fallback
        }
    }

    /// Convert distance to RGB color
    fn distance_to_color(&self, distance: f32, frame: u32) -> [u8; 3] {
        let t = (frame as f32 * 0.01).sin() * 0.5 + 0.5;
        let normalized_distance = 1.0 / (1.0 + distance.abs());
        
        let r = (normalized_distance * t * 255.0) as u8;
        let g = (normalized_distance * (1.0 - t) * 255.0) as u8;
        let b = ((1.0 - normalized_distance) * 255.0) as u8;
        
        [r, g, b]
    }

    /// Extract keyframes from timeline
    fn extract_keyframes(&self, project: &TimelineProject) -> Vec<(u32, f32)> {
        let frame_rate = self.settings.frame_rate;
        let mut keyframes = Vec::new();

        // Extract keyframes from all tracks
        for track in project.timeline().tracks() {
            let animation_track = project.animation_controller()
                .fractal_tracks()
                .find(|t| t.name() == track.name);
            
            if let Some(anim_track) = animation_track {
                for keyframe in anim_track.keyframes() {
                    let frame = (keyframe.time * frame_rate) as u32;
                    keyframes.push((frame, keyframe.time));
                }
            }
        }

        // Sort and deduplicate
        keyframes.sort_by_key(|(frame, _)| *frame);
        keyframes.dedup_by_key(|(frame, _)| *frame);

        keyframes
    }

    /// Combine individual frames into video
    fn combine_frames_to_video(&self, output_path: &str) -> Result<Vec<String>, Box<dyn std::error::Error>> {
        match self.settings.format {
            AnimationFormat::MP4 => {
                let output_file = format!("{}.mp4", output_path);
                self.encode_video_ffmpeg(&output_file)?;
                Ok(vec![output_file])
            }
            AnimationFormat::AVI => {
                let output_file = format!("{}.avi", output_path);
                self.encode_video_ffmpeg(&output_file)?;
                Ok(vec![output_file])
            }
            AnimationFormat::MOV => {
                let output_file = format!("{}.mov", output_path);
                self.encode_video_ffmpeg(&output_file)?;
                Ok(vec![output_file])
            }
            AnimationFormat::WEBM => {
                let output_file = format!("{}.webm", output_path);
                self.encode_video_ffmpeg(&output_file)?;
                Ok(vec![output_file])
            }
            _ => Err("Unsupported video format".into()),
        }
    }

    /// Create GIF from frames
    fn create_gif_from_frames(&self, output_path: &str) -> Result<Vec<String>, Box<dyn std::error::Error>> {
        let output_file = format!("{}.gif", output_path);
        
        // This would use gif library to combine frames
        // For now, placeholder implementation
        std::fs::write(&output_file, "GIF data would go here")?;
        
        Ok(vec![output_file])
    }

    /// Encode video using FFmpeg (simplified)
    fn encode_video_ffmpeg(&self, output_file: &str) -> Result<(), Box<dyn std::error::Error>> {
        // This would call FFmpeg to encode the video
        // For now, just create a placeholder file
        std::fs::write(output_file, "Video encoding would happen here")?;
        Ok(())
    }

    /// Export keyframes as JSON
    fn export_keyframes_json(&self, project: &TimelineProject, output_path: &str) -> Result<(), Box<dyn std::error::Error>> {
        let json_data = serde_json::json!({
            "animation": {
                "duration": self.settings.duration,
                "frame_rate": self.settings.frame_rate,
                "tracks": project.timeline().tracks(),
                "keyframes": self.extract_all_keyframes(project)
            }
        });

        let output_file = format!("{}.json", output_path);
        std::fs::write(&output_file, serde_json::to_string_pretty(&json_data)?)?;
        Ok(())
    }

    /// Export animation as FBX
    fn export_fbx_animation(&self, _project: &TimelineProject, _output_path: &str) -> Result<(), Box<dyn std::error::Error>> {
        // FBX export would require a library like `fbx`
        Err("FBX export not yet implemented".into())
    }

    /// Export animation as GLTF
    fn export_gltf_animation(&self, _project: &TimelineProject, _output_path: &str) -> Result<(), Box<dyn std::error::Error>> {
        // GLTF export would require a library like `gltf`
        Err("GLTF export not yet implemented".into())
    }

    /// Extract all keyframes from project
    fn extract_all_keyframes(&self, project: &TimelineProject) -> serde_json::Value {
        let mut keyframes_data = Vec::new();

        for track in project.timeline().tracks() {
            let animation_track = project.animation_controller()
                .fractal_tracks()
                .find(|t| t.name() == track.name);
            
            if let Some(anim_track) = animation_track {
                for keyframe in anim_track.keyframes() {
                    keyframes_data.push(serde_json::json!({
                        "track": track.name,
                        "time": keyframe.time,
                        "value": keyframe.value,
                        "easing": format!("{:?}", keyframe.easing)
                    }));
                }
            }
        }

        serde_json::Value::Array(keyframes_data)
    }

    /// Calculate average frame render time
    fn calculate_average_frame_time(&self, frame_count: u32) -> f32 {
        // Placeholder calculation
        0.016 * frame_count as f32 // Assume ~60 FPS
    }

    /// Calculate total render time
    fn calculate_total_render_time(&self, frame_count: u32) -> f32 {
        self.calculate_average_frame_time(frame_count)
    }

    /// Calculate total file size
    fn calculate_total_file_size(&self, files: &[String]) -> f32 {
        let mut total_size = 0u64;
        for file in files {
            if let Ok(metadata) = std::fs::metadata(file) {
                total_size += metadata.len();
            }
        }
        total_size as f32 / (1024.0 * 1024.0) // Convert to MB
    }
}

impl Default for AnimationExportSettings {
    fn default() -> Self {
        Self {
            format: AnimationFormat::MP4,
            frame_rate: 30.0,
            resolution: [1920, 1080],
            duration: 10.0,
            export_frames: ExportFrames::AllFrames,
            quality: AnimationQuality::Standard,
            compression: AnimationCompression::H264,
            include_audio: false,
        }
    }
}

/// Extension trait for AnimationController to expose fractal_tracks
trait FractalTracksExt {
    fn fractal_tracks(&self) -> &[crate::animation::keyframe::AnimationTrack<crate::fractal::types::FractalParameters>];
}

impl FractalTracksExt for crate::animation::keyframe::AnimationController {
    fn fractal_tracks(&self) -> &[crate::animation::keyframe::AnimationTrack<crate::fractal::types::FractalParameters>] {
        &self.fractal_tracks
    }
}