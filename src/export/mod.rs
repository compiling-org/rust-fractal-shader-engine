//! Export System Module
//!
//! This module handles exporting fractals to various formats including
//! images, videos, 3D meshes, and animation data.

use std::path::Path;
use std::fs::File;
use std::io::Write;
use image::{ImageBuffer, Rgba};
use nalgebra::Vector3;

/// Export error types
#[derive(Debug)]
pub enum ExportError {
    IoError(std::io::Error),
    ImageError(image::ImageError),
    JsonError(serde_json::Error),
    UnsupportedFormat,
    InvalidData,
}

impl From<std::io::Error> for ExportError {
    fn from(error: std::io::Error) -> Self {
        ExportError::IoError(error)
    }
}

impl From<image::ImageError> for ExportError {
    fn from(error: image::ImageError) -> Self {
        ExportError::ImageError(error)
    }
}

impl From<serde_json::Error> for ExportError {
    fn from(error: serde_json::Error) -> Self {
        ExportError::JsonError(error)
    }
}

/// Supported export formats
#[derive(Debug, Clone, Copy)]
pub enum ExportFormat {
    PNG,
    JPEG,
    TIFF,
    EXR,
    MP4,
    OBJ,
    FBX,
    GLTF,
}

/// Export settings
#[derive(Debug, Clone)]
pub struct ExportSettings {
    pub format: ExportFormat,
    pub width: u32,
    pub height: u32,
    pub quality: f32, // 0.0 - 1.0
    pub frame_rate: u32, // for video
    pub start_frame: u32,
    pub end_frame: u32,
}

/// Image export functionality
pub struct ImageExporter;

impl ImageExporter {
    /// Export RGBA pixel data to image file
    pub fn export_image(
        pixels: &[u8],
        width: u32,
        height: u32,
        format: ExportFormat,
        path: &Path,
    ) -> Result<(), ExportError> {
        match format {
            ExportFormat::PNG => {
                let img = ImageBuffer::<Rgba<u8>, _>::from_raw(width, height, pixels.to_vec())
                    .ok_or(ExportError::InvalidData)?;
                img.save_with_format(path, image::ImageFormat::Png)?;
            }
            ExportFormat::JPEG => {
                let img = ImageBuffer::<Rgba<u8>, _>::from_raw(width, height, pixels.to_vec())
                    .ok_or(ExportError::InvalidData)?;
                let quality = 90; // Default quality for JPEG
                let mut encoder = image::codecs::jpeg::JpegEncoder::new_with_quality(
                    File::create(path)?,
                    quality,
                );
                img.write_with_encoder(encoder)?;
            }
            ExportFormat::TIFF => {
                let img = ImageBuffer::<Rgba<u8>, _>::from_raw(width, height, pixels.to_vec())
                    .ok_or(ExportError::InvalidData)?;
                img.save_with_format(path, image::ImageFormat::Tiff)?;
            }
            ExportFormat::EXR => {
                // For EXR, we'd need additional dependencies
                // For now, fall back to PNG
                let img = ImageBuffer::<Rgba<u8>, _>::from_raw(width, height, pixels.to_vec())
                    .ok_or(ExportError::InvalidData)?;
                img.save_with_format(path.with_extension("png"), image::ImageFormat::Png)?;
            }
            _ => return Err(ExportError::UnsupportedFormat),
        }

        Ok(())
    }

    /// Export fractal distance field as image
    pub fn export_fractal_image(
        distance_field: &[f32],
        width: u32,
        height: u32,
        path: &Path,
        color_map: &crate::fractal::ColorMapping,
    ) -> Result<(), ExportError> {
        let mut pixels = Vec::with_capacity((width * height * 4) as usize);

        for &distance in distance_field {
            // Convert distance to color using the color mapping
            let t = (distance / 100.0).clamp(0.0, 1.0); // Normalize distance

            // Sample color palette
            let palette_index = (t * (color_map.palette.len() - 1) as f32) as usize;
            let color = color_map.palette[palette_index];

            // Apply color adjustments
            let r = ((color.x * color_map.brightness + color_map.contrast * (color.x - 0.5) + 0.5) * 255.0) as u8;
            let g = ((color.y * color_map.brightness + color_map.contrast * (color.y - 0.5) + 0.5) * 255.0) as u8;
            let b = ((color.z * color_map.brightness + color_map.contrast * (color.z - 0.5) + 0.5) * 255.0) as u8;

            pixels.extend_from_slice(&[r, g, b, 255]);
        }

        Self::export_image(&pixels, width, height, ExportFormat::PNG, path)
    }
}

/// Video export functionality
pub struct VideoExporter;

impl VideoExporter {
    /// Export animation frames to video
    pub fn export_video(
        frame_data: Vec<Vec<u8>>,
        width: u32,
        height: u32,
        frame_rate: u32,
        path: &Path,
    ) -> Result<(), ExportError> {
        // For now, export as image sequence
        // In a real implementation, you'd use ffmpeg or similar
        for (i, frame) in frame_data.iter().enumerate() {
            let frame_path = path.with_file_name(format!("frame_{:04}.png", i));
            ImageExporter::export_image(frame, width, height, ExportFormat::PNG, &frame_path)?;
        }

        // Create a simple batch file for ffmpeg (if available)
        let batch_path = path.with_extension("bat");
        let ffmpeg_cmd = format!(
            "ffmpeg -framerate {} -i frame_%04d.png -c:v libx264 -pix_fmt yuv420p \"{}\"",
            frame_rate,
            path.with_extension("mp4").display()
        );

        let mut batch_file = File::create(batch_path)?;
        writeln!(batch_file, "{}", ffmpeg_cmd)?;

        Ok(())
    }
}

/// 3D mesh export functionality
pub struct MeshExporter;

impl MeshExporter {
    /// Export fractal as OBJ mesh
    pub fn export_obj(
        vertices: &[Vector3<f32>],
        indices: &[u32],
        normals: &[Vector3<f32>],
        path: &Path,
    ) -> Result<(), ExportError> {
        let mut file = File::create(path)?;

        // Write vertices
        for vertex in vertices {
            writeln!(file, "v {} {} {}", vertex.x, vertex.y, vertex.z)?;
        }

        // Write normals
        for normal in normals {
            writeln!(file, "vn {} {} {}", normal.x, normal.y, normal.z)?;
        }

        // Write faces
        for chunk in indices.chunks(3) {
            if chunk.len() == 3 {
                // OBJ indices are 1-based
                writeln!(file, "f {} {} {}",
                    chunk[0] + 1, chunk[1] + 1, chunk[2] + 1)?;
            }
        }

        Ok(())
    }

    /// Export fractal as STL mesh (binary)
    pub fn export_stl(
        vertices: &[Vector3<f32>],
        indices: &[u32],
        path: &Path,
    ) -> Result<(), ExportError> {
        let mut file = File::create(path)?;

        // STL header (80 bytes)
        let header = [0u8; 80];
        file.write_all(&header)?;

        // Number of triangles (4 bytes, little endian)
        let num_triangles = (indices.len() / 3) as u32;
        file.write_all(&num_triangles.to_le_bytes())?;

        // Write triangles
        for chunk in indices.chunks(3) {
            if chunk.len() == 3 {
                // Normal vector (12 bytes) - compute face normal
                let v0 = vertices[chunk[0] as usize];
                let v1 = vertices[chunk[1] as usize];
                let v2 = vertices[chunk[2] as usize];

                let normal = (v1 - v0).cross(&(v2 - v0)).normalize();
                file.write_all(&normal.x.to_le_bytes())?;
                file.write_all(&normal.y.to_le_bytes())?;
                file.write_all(&normal.z.to_le_bytes())?;

                // Vertices (36 bytes)
                for &idx in chunk {
                    let vertex = vertices[idx as usize];
                    file.write_all(&vertex.x.to_le_bytes())?;
                    file.write_all(&vertex.y.to_le_bytes())?;
                    file.write_all(&vertex.z.to_le_bytes())?;
                }

                // Attribute byte count (2 bytes)
                file.write_all(&[0u8, 0u8])?;
            }
        }

        Ok(())
    }

    /// Generate mesh from fractal distance field using marching cubes
    pub fn generate_fractal_mesh(
        distance_field: &[f32],
        width: u32,
        height: u32,
        depth: u32,
        iso_level: f32,
    ) -> (Vec<Vector3<f32>>, Vec<u32>, Vec<Vector3<f32>>) {
        // Simplified marching cubes implementation
        // In a real implementation, this would be much more sophisticated

        let mut vertices = Vec::new();
        let mut indices = Vec::new();
        let mut normals = Vec::new();

        // For now, create a simple cube mesh as placeholder
        vertices.extend_from_slice(&[
            Vector3::new(-1.0, -1.0, -1.0),
            Vector3::new(1.0, -1.0, -1.0),
            Vector3::new(1.0, 1.0, -1.0),
            Vector3::new(-1.0, 1.0, -1.0),
            Vector3::new(-1.0, -1.0, 1.0),
            Vector3::new(1.0, -1.0, 1.0),
            Vector3::new(1.0, 1.0, 1.0),
            Vector3::new(-1.0, 1.0, 1.0),
        ]);

        indices.extend_from_slice(&[
            0, 1, 2, 2, 3, 0, // front
            1, 5, 6, 6, 2, 1, // right
            5, 4, 7, 7, 6, 5, // back
            4, 0, 3, 3, 7, 4, // left
            3, 2, 6, 6, 7, 3, // top
            4, 5, 1, 1, 0, 4, // bottom
        ]);

        // Simple normals for cube faces
        normals.extend_from_slice(&[
            Vector3::new(0.0, 0.0, -1.0), // front
            Vector3::new(1.0, 0.0, 0.0),  // right
            Vector3::new(0.0, 0.0, 1.0),  // back
            Vector3::new(-1.0, 0.0, 0.0), // left
            Vector3::new(0.0, 1.0, 0.0),  // top
            Vector3::new(0.0, -1.0, 0.0), // bottom
        ]);

        (vertices, indices, normals)
    }
}

/// Animation export functionality
pub struct AnimationExporter;

impl AnimationExporter {
    /// Export animation data as JSON
    pub fn export_animation_json(
        animation_data: &crate::animation::AnimationController,
        path: &Path,
    ) -> Result<(), ExportError> {
        // AnimationController doesn't implement Serialize, so we'll export basic info
        let animation_info = serde_json::json!({
            "playing": animation_data.is_playing(),
            "current_time": animation_data.current_time(),
        });
        let json = serde_json::to_string_pretty(&animation_info)?;
        let mut file = File::create(path)?;
        file.write_all(json.as_bytes())?;
        Ok(())
    }

    /// Export keyframe data for external animation software
    pub fn export_keyframes_csv(
        tracks: &std::collections::HashMap<crate::animation::keyframe::TrackId, crate::animation::keyframe::AnimationTrackType>,
        path: &Path,
    ) -> Result<(), ExportError> {
        let mut file = File::create(path)?;

        // Write CSV header
        writeln!(file, "Track ID,Track Name,Time,Value X,Value Y,Value Z,Interpolation")?;

        // Write keyframe data
        for (track_id, track_type) in tracks {
            match track_type {
                crate::animation::keyframe::AnimationTrackType::Float(track) => {
                    for keyframe in &track.keyframes {
                        writeln!(file, "{},{},{},{},{},{},{}",
                            track_id,
                            track.name,
                            keyframe.time,
                            keyframe.value,
                            "",
                            "",
                            format!("{:?}", keyframe.interpolation)
                        )?;
                    }
                }
                crate::animation::keyframe::AnimationTrackType::Vec3(track) => {
                    for keyframe in &track.keyframes {
                        writeln!(file, "{},{},{},{},{},{},{}",
                            track_id,
                            track.name,
                            keyframe.time,
                            keyframe.value.x,
                            keyframe.value.y,
                            keyframe.value.z,
                            format!("{:?}", keyframe.interpolation)
                        )?;
                    }
                }
                _ => {} // Handle other types as needed
            }
        }

        Ok(())
    }
}

/// Main export interface
pub struct Exporter;

impl Exporter {
    /// Export fractal with given settings
    pub fn export_fractal(
        fractal_data: &[f32],
        settings: &ExportSettings,
        path: &Path,
        color_map: &crate::fractal::ColorMapping,
    ) -> Result<(), ExportError> {
        match settings.format {
            ExportFormat::PNG | ExportFormat::JPEG | ExportFormat::TIFF | ExportFormat::EXR => {
                ImageExporter::export_fractal_image(
                    fractal_data,
                    settings.width,
                    settings.height,
                    path,
                    color_map,
                )
            }
            ExportFormat::OBJ => {
                // Generate mesh from distance field
                let (vertices, indices, normals) = MeshExporter::generate_fractal_mesh(
                    fractal_data,
                    settings.width,
                    settings.height,
                    32, // depth
                    0.0, // iso level
                );
                MeshExporter::export_obj(&vertices, &indices, &normals, path)
            }
            _ => Err(ExportError::UnsupportedFormat),
        }
    }

    /// Export scene with all objects
    pub fn export_scene(
        scene: &crate::scene::Scene,
        settings: &ExportSettings,
        path: &Path,
    ) -> Result<(), ExportError> {
        match settings.format {
            ExportFormat::GLTF => {
                // TODO: Implement glTF export
                Err(ExportError::UnsupportedFormat)
            }
            ExportFormat::OBJ => {
                // Export all mesh objects in scene
                for (id, object) in &scene.objects {
                    if let crate::scene::ObjectType::Mesh { vertices, indices, .. } = &object.object_type {
                        let object_path = path.with_file_name(format!("object_{}.obj", id));
                        MeshExporter::export_obj(vertices, indices, &[], &object_path)?;
                    }
                }
                Ok(())
            }
            _ => Err(ExportError::UnsupportedFormat),
        }
    }
}