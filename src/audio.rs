use std::sync::{Arc, Mutex};
use std::collections::HashMap;
use bevy::prelude::*;

/// Audio analysis data structure
#[derive(Debug, Clone, Resource)]
pub struct AudioData {
    pub spectrum: Vec<f32>,
    pub waveform: Vec<f32>,
    pub beat: f32,
    pub bass_level: f32,
    pub mid_level: f32,
    pub treble_level: f32,
    pub volume: f32,
}

/// Audio analyzer for real-time audio processing
#[derive(Resource)]
pub struct AudioAnalyzer {
    pub current_data: Arc<Mutex<AudioData>>,
    pub fft_size: usize,
    pub sample_rate: f32,
}

impl Default for AudioData {
    fn default() -> Self {
        Self {
            spectrum: vec![0.0; 256],
            waveform: vec![0.0; 1024],
            beat: 0.0,
            bass_level: 0.0,
            mid_level: 0.0,
            treble_level: 0.0,
            volume: 0.0,
        }
    }
}

impl AudioAnalyzer {
    pub fn new() -> Self {
        Self {
            current_data: Arc::new(Mutex::new(AudioData::default())),
            fft_size: 512,
            sample_rate: 44100.0,
        }
    }

    /// Process audio samples and update analysis data
    pub fn process_samples(&self, samples: &[f32]) {
        let mut data = self.current_data.lock().unwrap();

        // Simple FFT simulation (in real implementation, use rustfft)
        self.compute_fft(samples, &mut data.spectrum);

        // Update waveform
        data.waveform.clear();
        data.waveform.extend_from_slice(&samples[..samples.len().min(1024)]);

        // Compute frequency bands
        data.bass_level = self.compute_frequency_band(&data.spectrum, 0, 64);
        data.mid_level = self.compute_frequency_band(&data.spectrum, 64, 128);
        data.treble_level = self.compute_frequency_band(&data.spectrum, 128, 256);

        // Compute overall volume
        data.volume = samples.iter().map(|x| x.abs()).sum::<f32>() / samples.len() as f32;

        // Simple beat detection
        data.beat = self.detect_beat(&data.spectrum);
    }

    /// Compute FFT (simplified version)
    fn compute_fft(&self, samples: &[f32], spectrum: &mut Vec<f32>) {
        // This is a very simplified FFT - in practice, use a proper FFT library
        spectrum.clear();
        spectrum.resize(256, 0.0);

        for i in 0..256 {
            let frequency = (i as f32 / 256.0) * (self.sample_rate / 2.0);
            let mut real = 0.0;
            let mut imag = 0.0;

            for (j, &sample) in samples.iter().enumerate() {
                let angle = -2.0 * std::f32::consts::PI * frequency * j as f32 / self.sample_rate;
                real += sample * angle.cos();
                imag += sample * angle.sin();
            }

            spectrum[i] = (real * real + imag * imag).sqrt();
        }

        // Normalize
        let max_val = spectrum.iter().cloned().fold(0.0, f32::max);
        if max_val > 0.0 {
            for val in spectrum.iter_mut() {
                *val /= max_val;
            }
        }
    }

    /// Compute frequency band level
    fn compute_frequency_band(&self, spectrum: &[f32], start: usize, end: usize) -> f32 {
        let end = end.min(spectrum.len());
        let sum: f32 = spectrum[start..end].iter().sum();
        sum / (end - start) as f32
    }

    /// Simple beat detection
    fn detect_beat(&self, spectrum: &[f32]) -> f32 {
        // Simple beat detection based on bass frequencies
        let bass_energy = self.compute_frequency_band(spectrum, 0, 32);
        let mid_energy = self.compute_frequency_band(spectrum, 32, 64);

        // Return a value between 0 and 1 indicating beat strength
        (bass_energy + mid_energy * 0.5).min(1.0)
    }

    /// Get current audio data
    pub fn get_data(&self) -> AudioData {
        self.current_data.lock().unwrap().clone()
    }
}

/// MIDI controller for parameter mapping
#[derive(Resource)]
pub struct MidiController {
    pub mappings: HashMap<u8, MidiMapping>,
    pub current_values: HashMap<String, f32>,
}

#[derive(Debug, Clone)]
pub struct MidiMapping {
    pub parameter_name: String,
    pub channel: u8,
    pub controller: u8,
    pub min_value: f32,
    pub max_value: f32,
}

impl MidiController {
    pub fn new() -> Self {
        Self {
            mappings: HashMap::new(),
            current_values: HashMap::new(),
        }
    }

    /// Add a MIDI mapping
    pub fn add_mapping(&mut self, mapping: MidiMapping) {
        self.mappings.insert(mapping.controller, mapping);
    }

    /// Process MIDI message
    pub fn process_midi_message(&mut self, channel: u8, controller: u8, value: u8) {
        if let Some(mapping) = self.mappings.get(&controller) {
            if mapping.channel == channel {
                let normalized_value = value as f32 / 127.0;
                let mapped_value = mapping.min_value +
                    (mapping.max_value - mapping.min_value) * normalized_value;
                self.current_values.insert(mapping.parameter_name.clone(), mapped_value);
            }
        }
    }

    /// Get parameter value
    pub fn get_parameter(&self, name: &str) -> f32 {
        *self.current_values.get(name).unwrap_or(&0.0)
    }

    /// Create default mappings for common parameters
    pub fn create_default_mappings(&mut self) {
        let mappings = vec![
            MidiMapping {
                parameter_name: "speed".to_string(),
                channel: 0,
                controller: 1, // Mod wheel
                min_value: 0.0,
                max_value: 5.0,
            },
            MidiMapping {
                parameter_name: "zoom".to_string(),
                channel: 0,
                controller: 2,
                min_value: 0.2,
                max_value: 5.0,
            },
            MidiMapping {
                parameter_name: "iterations".to_string(),
                channel: 0,
                controller: 3,
                min_value: 10.0,
                max_value: 200.0,
            },
            MidiMapping {
                parameter_name: "brightness".to_string(),
                channel: 0,
                controller: 4,
                min_value: 0.0,
                max_value: 20.0,
            },
            MidiMapping {
                parameter_name: "contrast".to_string(),
                channel: 0,
                controller: 5,
                min_value: 0.0,
                max_value: 3.0,
            },
        ];

        for mapping in mappings {
            self.add_mapping(mapping);
        }
    }
}

/// Combined audio and MIDI system
#[derive(Resource)]
pub struct AudioMidiSystem {
    pub audio_analyzer: AudioAnalyzer,
    pub midi_controller: MidiController,
}

impl AudioMidiSystem {
    pub fn new() -> Self {
        let mut midi_controller = MidiController::new();
        midi_controller.create_default_mappings();

        Self {
            audio_analyzer: AudioAnalyzer::new(),
            midi_controller,
        }
    }

    /// Update system with new audio samples
    pub fn update_audio(&self, samples: &[f32]) {
        self.audio_analyzer.process_samples(samples);
    }

    /// Process MIDI message
    pub fn process_midi(&mut self, channel: u8, controller: u8, value: u8) {
        self.midi_controller.process_midi_message(channel, controller, value);
    }

    /// Get combined parameter value (audio + MIDI)
    pub fn get_parameter(&self, name: &str) -> f32 {
        let midi_value = self.midi_controller.get_parameter(name);
        let audio_data = self.audio_analyzer.get_data();

        // Combine MIDI and audio influences
        match name {
            "speed" => midi_value + audio_data.beat * 2.0,
            "brightness" => midi_value + audio_data.volume * 10.0,
            "zoom" => midi_value * (1.0 + audio_data.bass_level),
            _ => midi_value,
        }
    }

    /// Get current audio data
    pub fn get_audio_data(&self) -> AudioData {
        self.audio_analyzer.get_data()
    }
}