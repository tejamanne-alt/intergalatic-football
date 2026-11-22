# Art prompt sheet

Prompts for generating reference images in ChatGPT. I use the images to build the Blender scenes and animations: the stadium, the two species, the ball, the powers and the cinematic sequences.

The direction is **an artful escape**: dreamlike, theatrical and immersive, with football as a cosmic ritual. Every design here is original; the prompts steer away from existing games, films and brands.

## How to use this sheet

1. Start a new ChatGPT chat and paste the **style bible** below as your first message. Then paste prompts one at a time. Start a fresh chat for each wave, or whenever the style drifts.
2. Pick the image size each prompt asks for. ChatGPT offers square, landscape (3:2) and portrait (2:3).
3. Do **Wave 1 first**: it is everything I need for the first Blender scene. Waves 2 and 3 can come later.
4. For consistency, once you like an image, attach it to later prompts and add: *"Match the style and character of the attached image."* Do this especially for the turnaround sheets: attach the hero image of the same species.
5. If an image comes back with text, labels or a watermark, reply: *"Same image, remove all text."*
6. Send me the images in this chat, named by ID (`W1-04.png`), or tell me the ID when you attach them. Send any variants you like and tell me your favourite.

## Style bible (paste first in every chat)

```text
You are the concept artist for INTERGALACTIC FOOTBALL, an original turn-based football game played between alien species on a floating stadium in deep space. Every image you make follows this style bible.

FEELING: An artful escape. Dreamlike, theatrical, full of wonder. Football as a cosmic ritual and a celebration of light, color and music. Immersive: the viewer should want to step into every picture.

LOOK: Painterly, stylized 3D, like a frame from a hand-crafted animated film. Soft volumetric light, glowing haze, strong rim light, bioluminescence. Giant moons, ringed planets and nebulae in the sky. Rich saturated color fields against deep shadows. Never photoreal, never plastic or toy-like, never gritty.

SHAPES: Gravions are circles and heavy rounded masses, stone with molten light inside. Blinkers are hexagons, facets and light trails, translucent crystal. The stadium mixes both: floating stone rings and crystal arches.

PALETTE: Gravions amber #FFB02E, molten orange #E07A10, deep umber #2A1400. Blinkers cyan #41E3FF, ice #B4F6FF, violet #6A4DFF. Pitch teal #0F4A44. Sky indigo #22175A fading to near-black #05060F, with magenta-violet nebula glow.

RULES: Original designs only. Nothing that resembles existing video games, films, sports brands, mascots or franchise characters. No text, letters, numbers, logos or watermarks unless I ask for them.
```

Every prompt also ends with this short style line, so each one still works on its own:

> Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.

---

## Wave 1: the first Blender scene

What I build from this wave:

- the floating stadium, with the exact 15×9 grid from the rules engine
- one Gravion and one Blinker, modelled, rigged and animated
- the ball
- a short kick-off-to-goal sequence

### W1-01 Key art: the arrival · Landscape

Use: lighting, colour and atmosphere for the whole stadium scene; the opening shot of the flyover.

```text
Key art for Intergalactic Football. A floating football stadium drifts through a vast violet nebula: a circular island of dark stone ringed by slowly orbiting boulders, with a glowing teal pitch at its center. The pitch is rectangular, wider than it is deep, and divided into a subtle grid of square tiles traced by faint luminous seams in the turf. At each short end stands a goal shaped like an arch of light: amber on the left, cyan on the right. A giant ringed planet and two moons hang low in the sky, and beams of starlight pour down onto the pitch like stage lights. Small crowds of many different alien species fill floating terraces around the pitch, their handheld lanterns twinkling. Camera: high three-quarter view from beyond the edge of the island, the whole pitch visible, a feeling of awe and scale. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

### W1-02 Stadium plan, top-down · Landscape

Use: the layout of the arena around the pitch. I build the pitch grid exactly myself, so the tile count in this image doesn't matter.

```text
Top-down orthographic plan view of a floating football stadium, seen straight from above with no perspective. A rectangular teal pitch sits in the exact center, wider than it is deep (about 15 to 9), with a faint square-tile grid, a halfway line, a center circle and a penalty box at each short end. Goal arches stand at the two short ends: amber on the left, cyan on the right. Around the pitch, in rings: a dark stone walkway, then curved floating terrace segments for the crowd, then orbiting boulders at the edge of the island, then open space. Even, flat lighting, crisp readable shapes, like a beautifully painted architectural illustration. Style: painterly stylized 3D, rich saturated color, original design, no text, labels or logos.
```

### W1-03 Sky panorama · Landscape

Use: wrapped onto the sky dome around the whole scene.

```text
A wide panoramic sky for a 3D skydome, with no ground and no land: only deep space. A deep indigo-to-black sky filled with a swirling magenta-violet and cyan nebula, a giant ringed gas planet partly visible near one side, two small moons, scattered star clusters and soft rays of light. Calm, majestic and immersive. Keep the detail spread evenly across the full width so it can wrap around a scene, with the brightest nebula slightly above the middle. Style: painterly stylized 3D film still, dreamlike cosmic wonder, rich saturated color, original design, no text or logos.
```

### W1-04 Gravion hero · Portrait

Use: the character's personality, materials and colours.

```text
Character concept art: a Gravion footballer, an original alien species. Stout, broad and heavy, almost as wide as it is tall, with a low center of gravity and thick legs built for planting. Its body is dark basalt-like stone, smooth and rounded like river rock, with molten amber light glowing through fine cracks and seams. Three small pebbles orbit slowly around its shoulders. A calm, kind, determined face with deep-set glowing amber eyes and a wide jaw. It wears a ceremonial football kit: a deep umber wrap and sash with circular amber emblems, and heavy boots of layered stone. Pose: standing wide and planted, one foot resting on a small glowing ball, looking toward the viewer with quiet confidence. Background: a softly blurred floating stadium under a violet nebula. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

### W1-05 Gravion turnaround sheet · Landscape

Use: modelling reference in Blender. Attach W1-04 when you send this prompt.

```text
Character turnaround model sheet for the Gravion footballer (attached image, if any): a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, deep umber wrap and sash with circular amber emblems, heavy layered stone boots. Show the same character five times in a row, evenly spaced, at exactly the same scale and standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view, back view. Neutral A-pose: arms angled 45 degrees away from the body, legs slightly apart, hands relaxed, mouth closed, eyes open. Orthographic, no perspective distortion. Plain flat light-grey background, flat even studio lighting, no cast shadows, no props, no ground plane. Identical proportions and details in every view. Style: clean painterly stylized 3D, original design, no text, labels, arrows or logos.
```

### W1-05b Gravion front view for 3D · Square

Use: a clean single view. This is the best input if you want to try an image-to-3D tool yourself (see [Sending models](#sending-models)).

```text
Full-body front view of the Gravion footballer (attached image, if any): a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, deep umber wrap and sash with circular amber emblems, heavy layered stone boots. Neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, facing the camera straight on. The whole body is centered and fully visible with a margin all around. Plain flat light-grey background, flat even lighting, no shadows, no props. Style: clean painterly stylized 3D, original design, no text or logos.
```

### W1-06 Blinker hero · Portrait

```text
Character concept art: a Blinker footballer, an original alien species. Tall, slender and light-footed, with long limbs and a narrow waist. Its body is semi-translucent faceted crystal: hexagonal facets glowing cyan at the edges and deep violet inside, like a figure carved out of light. A small smooth head with two large luminous ice-blue eyes and a calm, curious expression. Faint after-images of its own silhouette trail behind it, as if it is in two places at once. It wears a flowing violet kit of ribbon-like cloth patterned with small hexagons, and light pointed boots. Pose: mid-stride on tiptoe, one foot flicking a small glowing ball, leaning forward with grace and speed. Background: a softly blurred floating stadium under a cyan and violet nebula. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

### W1-07 Blinker turnaround sheet · Landscape

Attach W1-06 when you send this prompt.

```text
Character turnaround model sheet for the Blinker footballer (attached image, if any): a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, light pointed boots. Show the same character five times in a row, evenly spaced, at exactly the same scale and standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view, back view. Neutral A-pose: arms angled 45 degrees away from the body, legs slightly apart, hands relaxed. No after-image trails in this sheet. Orthographic, no perspective distortion. Plain flat light-grey background, flat even studio lighting, no cast shadows, no props, no ground plane. Identical proportions and details in every view. Style: clean painterly stylized 3D, original design, no text, labels, arrows or logos.
```

### W1-07b Blinker front view for 3D · Square

```text
Full-body front view of the Blinker footballer (attached image, if any): a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, light pointed boots. Neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, facing the camera straight on, no after-images. The whole body is centered and fully visible with a margin all around. Plain flat light-grey background, flat even lighting, no shadows, no props. Style: clean painterly stylized 3D, original design, no text or logos.
```

### W1-08 The ball · Square

```text
Prop design sheet: the official ball of Intergalactic Football, a sphere the size of a football. A polished pearl-white shell built from interlocking hexagonal and circular panels (one shape for each species), with a soft star-like core glowing pale cyan-white through the seams between panels. Show the ball three times on one sheet: front view, top view, and in flight with a short luminous trail. Plain dark indigo background, soft studio lighting. Style: painterly stylized 3D, rich color, original design, no text or logos.
```

### W1-09 Pitch turf texture · Square

Use: tiled across the pitch as a material. I'll repair the seams if it doesn't tile perfectly.

```text
Seamless tileable texture, viewed straight down, flat and evenly lit, with no shadows and no perspective: alien turf for a cosmic football pitch. Short, soft, moss-like blades in deep teal (#0F4A44) and green-teal, with tiny bioluminescent cyan specks scattered sparsely. Subtle natural variation, with no obvious repeating features, no lines and no grid. The edges must tile seamlessly in both directions. No text.
```

### W1-10 Storyboard: kick-off to goal · Landscape

Use: the shots, framing and beats of the first animated sequence. I drive the player movement from a real match played by the rules engine and use this board for the cinematography.

```text
A six-panel cinematic storyboard in a 3 by 2 grid for an animated sequence in Intergalactic Football, painted in a loose but polished style. Panel 1: wide shot of the floating stadium at kick-off, stout amber-glowing stone Gravions and slender cyan crystal Blinkers lined up on the glowing grid pitch. Panel 2: a Gravion passes the glowing ball along the turf, leaving a trail of amber light. Panel 3: a Blinker leaps and intercepts, its crystal body flaring cyan. Panel 4: the Blinker bursts into hexagonal shards and reappears a few tiles ahead, after-images fading behind. Panel 5: a low dramatic angle as the Blinker strikes the ball toward the amber goal arch while the Gravion keeper dives. Panel 6: the goal arch erupts into ribbons of cyan light and the crowd's lanterns float up into the sky. Thin dark borders between panels. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text, numbers or logos.
```

---

## Wave 2: powers, ultimates and moments

Each power is a 4-stage strip so I can time the effect in Blender: build-up, release, peak, fade. All strips use the gameplay camera, a high three-quarter view over the grid pitch.

### W2-01 Gravity Well (Gravion) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, release, peak, fade. Effect: Gravity Well. A stout stone Gravion with molten amber cracks raises one heavy hand; space ripples around it like a pond; concentric rings of amber light warp the turf of the glowing grid pitch; the glowing ball is pulled off the foot of a startled slender cyan crystal Blinker and curves through the air toward the Gravion; the pebbles orbiting the Gravion's shoulders spin faster and faster. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-02 Shockwave (Gravion) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, release, peak, fade. Effect: Shockwave. A stout stone Gravion with molten amber cracks stamps one foot on the glowing grid pitch; a ring of molten amber force bursts outward from its feet, lifting glowing dust and blades of teal turf; two slender cyan crystal Blinkers next to it are pushed back one step, their bodies flickering; the ball pops loose from one of them. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-03 Meteor Strike (Gravion) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: wind-up, strike, flight, impact. Effect: Meteor Strike. A stout stone Gravion striker draws back and kicks the glowing ball so hard that it ignites into a small meteor wrapped in molten amber fire and shards of rock. It tears a straight, blazing path across the grid pitch toward a cyan goal arch, passing untouched through the shimmering zones of two crystal Blinker defenders. Only the Blinker keeper stands in the goal, bracing. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-04 Blink (Blinker) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, vanish, reappear, fade. Effect: Blink. A slender cyan crystal Blinker carrying the glowing ball dissolves into a swirl of hexagonal crystal shards and reappears three tiles away on the glowing grid pitch in a flash of cyan light. A chain of fading after-images links the two spots, and two stout amber stone Gravions turn their heads too late. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-05 Phase Pass (Blinker) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: wind-up, release, phasing, catch. Effect: Phase Pass. A slender cyan crystal Blinker passes the ball; the ball turns ghostly and translucent and streams as a cyan wisp straight through two stout amber stone Gravion defenders, leaving hexagonal ripples hanging in the air; a teammate catches it cleanly on the far side. Same high three-quarter game camera over the glowing grid pitch in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-06 Rift Tackle (Blinker) · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, rift opens, tackle, rift closes. Effect: Rift Tackle. A slender cyan crystal Blinker defender tears open a thin vertical rift of violet light in the air, steps through it, and emerges right beside a stout amber stone Gravion who is carrying the ball, sliding in to win it. The rift seals behind them in a shower of violet sparks. Same high three-quarter game camera over the glowing grid pitch in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-07 Ultimate: Crush · Landscape

```text
Key art for the Gravion ultimate, Crush. The whole Gravion team, stout stone beings with molten amber cracks, raise their arms together in the middle of the floating stadium. The sky darkens and a colossal dome of amber gravity presses down on the pitch. The slender cyan crystal Blinkers move as if wading through deep water: their light trails are slowed and stretched, and dust and pebbles hang frozen in the air. Heavy, epic, slow and beautiful. Wide cinematic shot. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

### W2-08 Ultimate: Fold Space · Landscape

```text
Key art for the Blinker ultimate, Fold Space. The glowing grid pitch folds like a sheet of paper along seams of hexagonal light, curling up toward the nebula sky in a wave. Two slender cyan crystal Blinkers swap places through the fold, each leaving a spiral of after-images. Stout amber stone Gravions stare up in wonder. Wide cinematic shot, playful and impossible. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

### W2-09 Gravion goal celebration · Landscape

```text
A horizontal 4-panel storyboard of a goal celebration by the Gravions, stout stone beings with molten amber cracks. Panel 1: the scorer plants both feet and the ground glows amber beneath them. Panel 2: teammates gather in a circle and press their heavy hands together. Panel 3: their shoulder pebbles rise and orbit above the group like a small planetary system. Panel 4: a slow, warm pulse of amber light ripples out across the stadium and the crowd's lanterns answer it. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-10 Blinker goal celebration · Landscape

```text
A horizontal 4-panel storyboard of a goal celebration by the Blinkers, slender translucent crystal beings glowing cyan and violet. Panel 1: the scorer spins on tiptoe, leaving a ring of after-images. Panel 2: they blink rapidly across the pitch in a zigzag of cyan flashes. Panel 3: the whole team appears in a hexagon formation, each player in a different place at once. Panel 4: they burst into a fountain of hexagonal light shards that drift up into the nebula like snow falling upward. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

### W2-11 Gravion squad line-up · Landscape

Use: telling the five roles apart by silhouette. Every squad has the same five roles.

```text
Line-up sheet of five Gravion footballers standing side by side, same species, same scale, on one shared ground line, front view, neutral A-pose, plain flat light-grey background, flat even lighting, no shadows. All are stout stone beings with molten amber cracks, glowing amber eyes and deep umber kits, but each role has a clearly different silhouette. Keeper: the largest, with broad stone gloves and a faint amber shield glow around the hands. Anchor: very wide shoulders, the heaviest build. Playmaker: balanced, with a circular amber visor over the eyes. Runner: the leanest, with longer legs. Striker: powerful legs, with one boot glowing molten hot. Style: clean painterly stylized 3D, original design, no text, labels or logos.
```

### W2-12 Blinker squad line-up · Landscape

```text
Line-up sheet of five Blinker footballers standing side by side, same species, same scale, on one shared ground line, front view, neutral A-pose, plain flat light-grey background, flat even lighting, no shadows. All are slender translucent crystal beings with hexagonal facets glowing cyan and violet, luminous ice-blue eyes and flowing violet kits, but each role has a clearly different silhouette. Keeper: the tallest, with long arms and hexagonal light plates on the forearms. Anchor: thicker crystal plating on the chest and shoulders. Playmaker: a halo of small floating hexagons around the head. Runner: the thinnest, with long legs and trailing ribbons. Striker: one leg made of brighter, denser crystal. Style: clean painterly stylized 3D, original design, no text, labels or logos.
```

### W2-13 Gravion crest · Square, transparent background

```text
Emblem for the Gravions team: a round crest of carved dark stone with a molten amber core and three small pebbles orbiting it. Bold, simple shapes that read clearly at small sizes, softly painted. Centered on a transparent background. No text or letters.
```

### W2-14 Blinker crest · Square, transparent background

```text
Emblem for the Blinkers team: a hexagonal crest of cyan and violet crystal with a fast motif of two overlapping after-image outlines. Bold, simple shapes that read clearly at small sizes, softly painted. Centered on a transparent background. No text or letters.
```

---

## Wave 3: materials and set pieces

### W3-01 Gravion stone skin · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit with no shadows or perspective: smooth dark basalt stone like polished river rock, with fine branching cracks glowing molten amber (#FFB02E) and orange (#E07A10) from within. The cracks are sparse and elegant, not chaotic. The edges must tile seamlessly in both directions. No text.
```

### W3-02 Blinker crystal · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit: translucent faceted crystal made of irregular hexagonal facets, with cyan (#41E3FF) light along the facet edges and deep violet (#6A4DFF) in the depths, plus faint internal sparkles. The edges must tile seamlessly in both directions. No text.
```

### W3-03 Stadium stone walkway · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit: ancient dark stone paving for a floating stadium, made of large rounded slabs with thin seams of soft teal moss and an occasional tiny cyan bioluminescent flower. The edges must tile seamlessly in both directions. No text.
```

### W3-04 Goal arch design · Landscape

```text
Prop design sheet: the goal of Intergalactic Football. An arch three tiles wide made of two curved pillars, part dark stone and part crystal, joined at the top, with a shimmering curtain of light hanging between them as the net. Show an amber version and a cyan version, each in front view and side view, four drawings evenly spaced on a plain dark indigo background with soft studio lighting. Style: painterly stylized 3D, rich saturated color, original design, no text, labels or logos.
```

### W3-05 Crowd terraces close-up · Landscape

```text
A close view of one floating crowd terrace at the edge of the Intergalactic Football stadium: a curved slab of stone drifting in space, crowded with many different small alien species of all shapes and colors holding glowing lanterns, banners of pure colored light with no writing, joyful and strange, lit warmly by the glow of the pitch below. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

---

## Sending models

You can also send 3D models, from an image-to-3D tool (the W1-05b and W1-07b front views are the best inputs) or from an artist. Models are optional: I can build the characters myself from the turnaround sheets.

- **Format:** `.glb` (best, with textures embedded) or `.fbx` with its texture files. `.obj` works with its `.mtl` and textures.
- **Pose:** A-pose or T-pose, standing on the ground plane.
- **Rigging:** not needed. I'll rig and animate in Blender. If a model comes with a humanoid rig, even better.
- **Size for mobile:** under about 15,000 triangles per character, with textures at 2048 px or smaller. If a model is heavier, send it anyway and I'll reduce it.

## What I'll make in Blender

1. **The stadium.** Modelled from W1-01 and W1-02, with the exact 15×9 grid, penalty areas and goals from the rules engine. Sky from W1-03 and turf from W1-09.
2. **The characters.** A Gravion and a Blinker built from the turnarounds (or your models), then rigged, with idle, run, pass, shoot and celebrate animations.
3. **"Arrival":** a 10-second camera flight from the nebula down onto the pitch at kick-off.
4. **"First goal":** a 15–20 second sequence. The player movement comes from a real match replay from the rules engine; the shots and mood come from W1-10.
5. **Power effects:** short loops of each Wave 2 power for the game's UI and trailer.
6. **Deliverables:** MP4 video, the `.blend` files, and `.glb` exports ready for Unity.

Rendering here runs on CPU only, at about 20–60 seconds per 720p frame depending on how much haze and glow a scene has. A 15-second shot therefore takes two to six hours. I'll send quick low-resolution drafts first, so you can steer before the final renders.
