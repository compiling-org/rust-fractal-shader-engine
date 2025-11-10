use std::fs;
use std::path::{Path, PathBuf};
use std::process::exit;

use clap::{Parser, ValueEnum};
use regex::Regex;
use serde::Serialize;
use walkdir::WalkDir;

#[derive(ValueEnum, Clone, Debug)]
enum OutputFormat { Text, Json }

#[derive(Parser, Debug)]
#[command(name = "ui_analyzer", version, about = "Static Bevy/egui UI analyzer (no UI launch)")]
struct Args {
    /// Output format: text or json
    #[arg(long, value_enum, default_value_t = OutputFormat::Text)]
    format: OutputFormat,

    /// Write report to file; defaults to stdout
    #[arg(long)]
    output: Option<PathBuf>,

    /// Strict mode: treat warnings as errors and exit non-zero
    #[arg(long, default_value_t = false)]
    strict: bool,

    /// Max directory depth when scanning src/ui
    #[arg(long, default_value_t = 2)]
    max_depth: usize,
}

#[derive(Serialize, Clone, Copy, Debug)]
enum Severity { Pass, Info, Warn, Fail }

#[derive(Serialize, Debug)]
struct Finding {
    severity: Severity,
    message: String,
    path: Option<String>,
    lines: Vec<usize>,
}

fn main() {
    let args = Args::parse();
    let project_root = PathBuf::from(env!("CARGO_MANIFEST_DIR"));
    let src_dir = project_root.join("src");

    let mut findings: Vec<Finding> = Vec::new();

    // Define target files/directories to inspect
    let targets = vec![
        src_dir.join("gui.rs"),
        src_dir.join("ui").join("main.rs"),
        src_dir.join("ui").join("fractal_ui.rs"),
        src_dir.join("ui").join("panels.rs"),
        src_dir.join("fractal").join("renderer.rs"),
    ];

    // Collect contents where available
    let mut files: Vec<(PathBuf, String)> = Vec::new();
    for t in targets {
        if t.exists() {
            match fs::read_to_string(&t) {
                Ok(s) => files.push((t.clone(), s)),
                Err(e) => findings.push(Finding { severity: Severity::Warn, message: format!("Could not read {}: {}", t.display(), e), path: Some(t.display().to_string()), lines: vec![] }),
            }
        } else {
            findings.push(Finding { severity: Severity::Info, message: format!("Skipping missing file: {}", t.display()), path: Some(t.display().to_string()), lines: vec![] });
        }
    }

    // Helper to find occurrences in a file
    let find = |content: &str, re: &Regex| content.lines().enumerate().filter_map(|(i, l)| {
        if re.is_match(l) { Some(i + 1) } else { None }
    }).collect::<Vec<usize>>();

    // 1) gui.rs checks
    if let Some((path, content)) = files.iter().find(|(p, _)| p.ends_with("gui.rs")) {
        let re_viewport_window = Regex::new(r#"egui::Window::new\(\s*\"Viewport\"\s*\)"#).unwrap();
        let matches = find(content, &re_viewport_window);
        if matches.is_empty() {
            findings.push(Finding { severity: Severity::Pass, message: "No duplicate egui Viewport window".into(), path: Some(path.display().to_string()), lines: vec![] });
        } else {
            findings.push(Finding { severity: Severity::Fail, message: "Duplicate egui::Window('Viewport') may conflict with CentralPanel".into(), path: Some(path.display().to_string()), lines: matches });
        }

        let re_img_set = Regex::new(r#"img\.data\s*=\s*([Ss]ome\s*\()"#).unwrap();
        let re_img_any = Regex::new(r#"img\.data\s*="#).unwrap();
        let any_set = find(content, &re_img_any);
        let proper_set = find(content, &re_img_set);
        if !any_set.is_empty() && proper_set.len() == any_set.len() {
            findings.push(Finding { severity: Severity::Pass, message: "Bevy Image.data set via Some(..)".into(), path: Some(path.display().to_string()), lines: proper_set });
        } else if !any_set.is_empty() {
            findings.push(Finding { severity: Severity::Fail, message: "Image.data assignments missing Some(..) Option wrapper".into(), path: Some(path.display().to_string()), lines: any_set });
        }

        // ctx retrieval guard
        let re_ctx_get = Regex::new(r#"egui_contexts\.ctx_mut\("#).unwrap();
        let re_ctx_guard = Regex::new(r#"if\s+let\s+Some\(ctx\)\s*=\s*egui_contexts\.ctx_mut\("#).unwrap();
        let ctx_calls = find(content, &re_ctx_get);
        let guarded_calls = find(content, &re_ctx_guard);
        if !ctx_calls.is_empty() && !guarded_calls.is_empty() {
            findings.push(Finding { severity: Severity::Pass, message: "egui context retrieval is guarded".into(), path: Some(path.display().to_string()), lines: guarded_calls });
        } else if !ctx_calls.is_empty() {
            findings.push(Finding { severity: Severity::Warn, message: "egui context retrieval found; guard Option to avoid panics".into(), path: Some(path.display().to_string()), lines: ctx_calls });
        }

        // Bevy EguiPlugin insertion
        let re_egui_plugin = Regex::new(r#"EguiPlugin"#).unwrap();
        let ep = find(content, &re_egui_plugin);
        if ep.is_empty() {
            findings.push(Finding { severity: Severity::Warn, message: "No EguiPlugin insertion detected in gui.rs; ensure bevy_egui is added".into(), path: Some(path.display().to_string()), lines: vec![] });
        } else {
            findings.push(Finding { severity: Severity::Pass, message: "EguiPlugin reference present".into(), path: Some(path.display().to_string()), lines: ep });
        }
    }

    // 2) ui/main.rs central panel and viewport integration
    if let Some((path, content)) = files.iter().find(|(p, _)| p.ends_with(Path::new("ui").join("main.rs"))) {
        let re_central_panel = Regex::new(r#"egui::CentralPanel::default\(\)"#).unwrap();
        let re_show_viewport = Regex::new(r#"show_fractal_viewport\("#).unwrap();
        let cp = find(content, &re_central_panel);
        let sv = find(content, &re_show_viewport);
        if !cp.is_empty() && !sv.is_empty() {
            findings.push(Finding { severity: Severity::Pass, message: "CentralPanel calls show_fractal_viewport".into(), path: Some(path.display().to_string()), lines: sv });
        } else {
            findings.push(Finding { severity: Severity::Fail, message: "Missing CentralPanel or show_fractal_viewport; viewport may not render".into(), path: Some(path.display().to_string()), lines: vec![] });
        }

        // WGPU support and initializer
        let re_has_wgpu_support = Regex::new(r#"has_wgpu_support\s*:\s*bool"#).unwrap();
        let re_init_wgpu = Regex::new(r#"initialize_wgpu\("#).unwrap();
        let has_flag = find(content, &re_has_wgpu_support);
        let has_init = find(content, &re_init_wgpu);
        if !has_flag.is_empty() && !has_init.is_empty() {
            findings.push(Finding { severity: Severity::Pass, message: "WGPU support flag and initializer present".into(), path: Some(path.display().to_string()), lines: has_init });
        } else {
            findings.push(Finding { severity: Severity::Warn, message: "Missing WGPU support flag or initializer; GPU viewport may be fragile".into(), path: Some(path.display().to_string()), lines: vec![] });
        }

        // Risky patterns: thread::sleep in update
        let re_sleep = Regex::new(r#"std::thread::sleep\("#).unwrap();
        let sleeps = find(content, &re_sleep);
        if !sleeps.is_empty() {
            findings.push(Finding { severity: Severity::Warn, message: "std::thread::sleep detected in UI; can freeze frames".into(), path: Some(path.display().to_string()), lines: sleeps });
        }

        // Request repaint presence (informational)
        let re_repaint = Regex::new(r#"request_repaint\("#).unwrap();
        let repaints = find(content, &re_repaint);
        if repaints.is_empty() {
            findings.push(Finding { severity: Severity::Info, message: "No request_repaint calls detected; Bevy egui may rely on frame events".into(), path: Some(path.display().to_string()), lines: vec![] });
        }
    }

    // 3) fractal/renderer.rs render methods
    if let Some((path, content)) = files.iter().find(|(p, _)| p.ends_with(Path::new("fractal").join("renderer.rs"))) {
        let re_render_view = Regex::new(r#"render_frame_to_view\("#).unwrap();
        let re_render_tex = Regex::new(r#"render_frame_to_texture\("#).unwrap();
        let v = find(content, &re_render_view);
        let t = find(content, &re_render_tex);
        let v_nonempty = !v.is_empty();
        let t_nonempty = !t.is_empty();
        if v_nonempty { findings.push(Finding { severity: Severity::Pass, message: "render_frame_to_view present".into(), path: Some(path.display().to_string()), lines: v.clone() }); }
        if t_nonempty { findings.push(Finding { severity: Severity::Pass, message: "render_frame_to_texture present".into(), path: Some(path.display().to_string()), lines: t.clone() }); }
        if !v_nonempty && !t_nonempty {
            findings.push(Finding { severity: Severity::Fail, message: "Renderer missing viewport render methods".into(), path: Some(path.display().to_string()), lines: vec![] });
        }
    }

    // 4) General scan for risky unwraps in UI and gui
    for dir in [src_dir.join("ui"), src_dir.clone()] {
        if dir.exists() {
            for entry in WalkDir::new(&dir).min_depth(1).max_depth(args.max_depth) {
                let entry = match entry { Ok(e) => e, Err(_) => continue };
                if entry.file_type().is_file() {
                    let p = entry.path().to_path_buf();
                    if p.extension().and_then(|s| s.to_str()) == Some("rs") {
                        if let Ok(s) = fs::read_to_string(&p) {
                            let re_unwrap = Regex::new(r#"\.unwrap\(\)"#).unwrap();
                            let hits = s.lines().enumerate().filter_map(|(i, l)| {
                                if re_unwrap.is_match(l) { Some(i + 1) } else { None }
                            }).collect::<Vec<_>>();
                            if !hits.is_empty() {
                                findings.push(Finding { severity: Severity::Warn, message: "unwrap() calls found; prefer graceful handling".into(), path: Some(p.display().to_string()), lines: hits });
                            }
                        }
                    }
                }
            }
        }
    }

    // 5) TextureId reuse heuristics across UI files
    let mut texture_id_uses = 0usize;
    let ui_dir = src_dir.join("ui");
    if ui_dir.exists() {
        for entry in WalkDir::new(&ui_dir).min_depth(1).max_depth(args.max_depth) {
            let entry = match entry { Ok(e) => e, Err(_) => continue };
            if entry.file_type().is_file() {
                let p = entry.path().to_path_buf();
                if p.extension().and_then(|s| s.to_str()) == Some("rs") {
                    if let Ok(s) = fs::read_to_string(&p) {
                        let re_tex = Regex::new(r#"TextureId"#).unwrap();
                        let re_ui_image = Regex::new(r#"ui\.image\("#).unwrap();
                        let hits = find(&s, &re_tex).len() + find(&s, &re_ui_image).len();
                        texture_id_uses += hits;
                    }
                }
            }
        }
    }
    if texture_id_uses > 10 { // heuristic threshold
        findings.push(Finding { severity: Severity::Warn, message: format!("High TextureId/ui.image usage count: {}. Ensure no concurrent reuse of same ID across panels/windows.", texture_id_uses), path: None, lines: vec![] });
    }

    // Output
    match args.format {
        OutputFormat::Text => {
            println!("UI Analyzer Report (static checks, no UI launch):\n");
            for f in &findings {
                let pfx = match f.severity { Severity::Pass => "[PASS]", Severity::Info => "[INFO]", Severity::Warn => "[WARN]", Severity::Fail => "[FAIL]" };
                let loc = f.path.clone().unwrap_or_default();
                let in_loc = if loc.is_empty() { String::new() } else { format!(" in {}", loc) };
                let lines_str = if f.lines.is_empty() { String::new() } else { format!(" at lines {:?}", f.lines) };
                println!("{} {}{}{}", pfx, f.message, in_loc, lines_str);
            }
            println!("\nTips:\n- Ensure viewport is drawn only once (CentralPanel).\n- Guard egui ctx retrieval and Image handles; avoid unwrap() in UI path.\n- Avoid std::thread::sleep in UI; use timers/resources.\n- Log WGPU init failures and provide a retry.");
        }
        OutputFormat::Json => {
            let json = serde_json::to_string_pretty(&findings).unwrap();
            if let Some(path) = args.output.clone() {
                if let Err(e) = fs::write(&path, &json) {
                    eprintln!("Failed to write JSON report to {}: {}", path.display(), e);
                } else {
                    println!("Wrote JSON report to {}", path.display());
                }
            } else {
                println!("{}", json);
            }
        }
    }

    // Exit code for CI gating
    let mut fails = 0usize;
    let mut warns = 0usize;
    for f in &findings {
        match f.severity { Severity::Fail => fails += 1, Severity::Warn => warns += 1, _ => {} }
    }
    if fails > 0 || (args.strict && warns > 0) {
        exit(1);
    }
}