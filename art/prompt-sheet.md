# Art prompt sheet

Prompts for generating reference images in **ChatGPT** or **Gemini**. Every prompt comes in a version tuned for each tool; use either, or both. I use the images to build the Blender scenes and animations: the stadium, the two species, the ball, the powers and the cinematic sequences.

The direction is **an artful escape**: dreamlike, theatrical and immersive, with football as a cosmic ritual. Every design here is original; the prompts steer away from existing games, films and brands.

## How to use this sheet

**In ChatGPT**

1. Start a new chat and paste the **ChatGPT style bible**. Wait for "Ready", then paste prompts one at a time.
2. Pick the size each prompt lists: square, landscape (3:2) or portrait (2:3).
3. For consistency, attach an image you like to later prompts and add: *"Match the style and character of the attached image."* Always attach the hero image when you make a turnaround sheet.
4. If an image comes back with text, labels or a watermark, reply: *"Same image, remove all text."*

**In Gemini**

1. Start a new chat in Gemini and paste the **Gemini style bible**. Wait for "Ready", then paste prompts one at a time.
2. Each Gemini prompt names its aspect ratio (16:9, 21:9, 2:3 or 1:1). If an image comes back square anyway, reply: *"Make this a 16:9 landscape image."* (or whichever ratio the prompt asks for).
3. Gemini is good at keeping a character the same across images. Attach the hero image you chose when you make a turnaround sheet. To refine an image, ask for one change at a time: *"Keep everything the same, but make the cracks glow brighter."*
4. Gemini can't make transparent backgrounds, so the crest prompts ask for flat white and I'll cut them out.
5. If an image has a small Gemini mark in a corner, send it anyway. I'll remove it.

**Either way**

- Do **Wave 1 first**: it is everything I need for the first Blender scene. Waves 2 and 3 can come later.
- Send me the images in this chat, named by ID (`W1-04.png`), or tell me the ID when you attach them. If you try both tools, send both and tell me which you prefer.

## Style bibles (paste first in every chat)

**ChatGPT**

```text
You are the concept artist for INTERGALACTIC FOOTBALL, an original turn-based football game played between alien species on a floating stadium in deep space. Every image you make follows this style bible.

FEELING: An artful escape. Dreamlike, theatrical, full of wonder. Football as a cosmic ritual and a celebration of light, color and music. Immersive: the viewer should want to step into every picture.

LOOK: Painterly, stylized 3D, like a frame from a hand-crafted animated film. Soft volumetric light, glowing haze, strong rim light, bioluminescence. Giant moons, ringed planets and nebulae in the sky. Rich saturated color fields against deep shadows. Never photoreal, never plastic or toy-like, never gritty.

SHAPES: Gravions are circles and heavy rounded masses, stone with molten light inside. Blinkers are hexagons, facets and light trails, translucent crystal. The stadium mixes both: floating stone rings and crystal arches.

PALETTE: Gravions amber #FFB02E, molten orange #E07A10, deep umber #2A1400. Blinkers cyan #41E3FF, ice #B4F6FF, violet #6A4DFF. Pitch teal #0F4A44. Sky indigo #22175A fading to near-black #05060F, with magenta-violet nebula glow.

RULES: Original designs only. Nothing that resembles existing video games, films, sports brands, mascots or franchise characters. No text, letters, numbers, logos or watermarks unless I ask for them.

Reply "Ready" and wait for my first prompt.
```

**Gemini** (written as a narrative, which Gemini follows best)

```text
I'm making concept art for INTERGALACTIC FOOTBALL, an original turn-based football game played between alien species on a floating stadium in deep space. Please follow this style guide for every image in this conversation.

The feeling is an artful escape: dreamlike, theatrical and full of wonder, with football as a cosmic ritual and a celebration of light, color and music. Every picture should feel immersive, like a place the viewer wants to step into.

Render everything as a painterly, stylized 3D frame from a hand-crafted animated film: soft volumetric light, glowing haze, strong rim light and bioluminescence, with giant moons, ringed planets and nebulae in the sky, and rich saturated color fields against deep shadows. Surfaces look handmade and painterly rather than photographic or plastic.

The two species each have their own shape language. Gravions are circles and heavy rounded masses: dark stone with molten amber light inside. Blinkers are hexagons, facets and trails of light: translucent cyan and violet crystal. The stadium combines both, with floating stone rings and crystal arches.

Palette: Gravions in amber #FFB02E, molten orange #E07A10 and deep umber #2A1400. Blinkers in cyan #41E3FF, ice #B4F6FF and violet #6A4DFF. The pitch is teal #0F4A44. The sky runs from indigo #22175A to near-black #05060F with a magenta-violet nebula glow.

All designs are original creations of this project, distinct from any existing game, film, sports brand or franchise. Each image is pure artwork with no lettering, numbers, logos or watermarks unless I ask for them.

Reply "Ready" and wait for my first prompt.
```

---

## Wave 1: the first Blender scene

What I build from this wave:

- the floating stadium, with the exact 15×9 grid from the rules engine
- one Gravion and one Blinker, modelled, rigged and animated
- the ball
- a short kick-off-to-goal sequence

### W1-01 Key art: the arrival

Use: lighting, colour and atmosphere for the whole stadium scene; the opening shot of the flyover.

**ChatGPT** · Landscape

```text
Key art for Intergalactic Football. A floating football stadium drifts through a vast violet nebula: a circular island of dark stone ringed by slowly orbiting boulders, with a glowing teal pitch at its center. The pitch is rectangular, wider than it is deep, and divided into a subtle grid of square tiles traced by faint luminous seams in the turf. At each short end stands a goal shaped like an arch of light: amber on the left, cyan on the right. A giant ringed planet and two moons hang low in the sky, and beams of starlight pour down onto the pitch like stage lights. Small crowds of many different alien species fill floating terraces around the pitch, their handheld lanterns twinkling. Camera: high three-quarter view from beyond the edge of the island, the whole pitch visible, a feeling of awe and scale. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 cinematic key art image for Intergalactic Football. We look down from a high three-quarter angle, from just beyond the edge of a floating stone island drifting through a vast violet nebula, with boulders orbiting slowly around its rim. At its center lies a glowing teal football pitch, wider than it is deep, its turf divided into a gentle grid of square tiles traced by faint luminous seams. At each short end of the pitch stands a goal shaped like an arch of light, amber on the left and cyan on the right. A giant ringed planet and two moons hang low in the sky, and beams of starlight fall onto the pitch like stage lights at a concert. Floating terraces around the pitch hold small crowds of many different alien species, their handheld lanterns twinkling. The whole pitch is visible, and the mood is awe and wonder at a vast scale. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W1-02 Stadium plan, top-down

Use: the layout of the arena around the pitch. I build the pitch grid exactly myself, so the tile count in this image doesn't matter.

**ChatGPT** · Landscape

```text
Top-down orthographic plan view of a floating football stadium, seen straight from above with no perspective. A rectangular teal pitch sits in the exact center, wider than it is deep (about 15 to 9), with a faint square-tile grid, a halfway line, a center circle and a penalty box at each short end. Goal arches stand at the two short ends: amber on the left, cyan on the right. Around the pitch, in rings: a dark stone walkway, then curved floating terrace segments for the crowd, then orbiting boulders at the edge of the island, then open space. Even, flat lighting, crisp readable shapes, like a beautifully painted architectural illustration. Style: painterly stylized 3D, rich saturated color, original design, no text, labels or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 top-down plan of a floating football stadium, seen from directly overhead like an architect's painted plan, with every line parallel to the frame. In the exact center lies a rectangular teal pitch, about 15 units wide and 9 deep, marked with a faint square-tile grid, a halfway line, a center circle and a penalty box at each short end. An amber goal arch stands at the left end and a cyan goal arch at the right. Around the pitch, in concentric rings, come a dark stone walkway, curved floating terrace segments for the crowd, orbiting boulders at the island's edge, and then open space. The lighting is flat and even so every shape reads crisply. Painterly stylized 3D surfaces with rich saturated color. An original design; pure artwork with no labels, lettering or logos.
```

### W1-03 Sky panorama

Use: wrapped onto the sky dome around the whole scene.

**ChatGPT** · Landscape

```text
A wide panoramic sky for a 3D skydome, with no ground and no land: only deep space. A deep indigo-to-black sky filled with a swirling magenta-violet and cyan nebula, a giant ringed gas planet partly visible near one side, two small moons, scattered star clusters and soft rays of light. Calm, majestic and immersive. Keep the detail spread evenly across the full width so it can wrap around a scene, with the brightest nebula slightly above the middle. Style: painterly stylized 3D film still, dreamlike cosmic wonder, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 panoramic sky for a 3D skydome, showing only deep space with no ground or land anywhere. A deep indigo sky fading to black is filled with a swirling magenta-violet and cyan nebula, its brightest region slightly above the middle. A giant ringed gas planet rises near one side, with two small moons, scattered star clusters and soft rays of light. The detail is spread evenly across the full width so the image can wrap around a scene. The mood is calm, majestic and immersive. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W1-04 Gravion hero

Use: the character's personality, materials and colours.

**ChatGPT** · Portrait

```text
Character concept art: a Gravion footballer, an original alien species. Stout, broad and heavy, almost as wide as it is tall, with a low center of gravity and thick legs built for planting. Its body is dark basalt-like stone, smooth and rounded like river rock, with molten amber light glowing through fine cracks and seams. Three small pebbles orbit slowly around its shoulders. A calm, kind, determined face with deep-set glowing amber eyes and a wide jaw. It wears a ceremonial football kit: a deep umber wrap and sash with circular amber emblems, and heavy boots of layered stone. Pose: standing wide and planted, one foot resting on a small glowing ball, looking toward the viewer with quiet confidence. Background: a softly blurred floating stadium under a violet nebula. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 2:3

```text
Create a 2:3 portrait of character concept art for a Gravion footballer, an original alien species. The Gravion is stout, broad and heavy, nearly as wide as it is tall, with a low center of gravity and thick legs built for planting. Its body is dark basalt-like stone, smooth and rounded like river rock, and molten amber light glows through fine cracks and seams across it. Three small pebbles orbit slowly around its shoulders. Its face is calm, kind and determined, with deep-set glowing amber eyes and a wide jaw. It wears a ceremonial football kit: a deep umber wrap and sash with circular amber emblems, and heavy boots of layered stone. It stands wide and planted with one foot resting on a small glowing ball, looking at us with quiet confidence. Behind it, a floating stadium under a violet nebula is softly out of focus, as if shot with a long lens. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W1-05 Gravion turnaround sheet

Use: modelling reference in Blender. Attach W1-04 when you send this prompt.

**ChatGPT** · Landscape

```text
Character turnaround model sheet for the Gravion footballer (attached image, if any): a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, deep umber wrap and sash with circular amber emblems, heavy layered stone boots. Show the same character five times in a row, evenly spaced, at exactly the same scale and standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view, back view. Neutral A-pose: arms angled 45 degrees away from the body, legs slightly apart, hands relaxed, mouth closed, eyes open. Orthographic, no perspective distortion. Plain flat light-grey background, flat even studio lighting, no cast shadows, no props, no ground plane. Identical proportions and details in every view. Style: clean painterly stylized 3D, original design, no text, labels, arrows or logos.
```

**Gemini** · 21:9

```text
Using the attached image as the character reference, create an ultra-wide 21:9 character turnaround model sheet of this Gravion footballer. If no image is attached, the character is a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, a deep umber wrap and sash with circular amber emblems, and heavy layered stone boots. Show the same character five times in a row, evenly spaced, at exactly the same scale, standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view and back view. In every view it holds a neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, hands relaxed, mouth closed and eyes open. Each view is orthographic with no perspective distortion, and the proportions and details match exactly from view to view. The background is plain flat light grey, lit by even studio light, with the figures standing on nothing and casting no shadows. Clean painterly stylized 3D rendering. An original design; pure artwork with no labels, arrows, lettering or logos.
```

### W1-05b Gravion front view for 3D

Use: a clean single view. This is the best input if you want to try an image-to-3D tool yourself (see [Sending models](#sending-models)).

**ChatGPT** · Square

```text
Full-body front view of the Gravion footballer (attached image, if any): a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, deep umber wrap and sash with circular amber emblems, heavy layered stone boots. Neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, facing the camera straight on. The whole body is centered and fully visible with a margin all around. Plain flat light-grey background, flat even lighting, no shadows, no props. Style: clean painterly stylized 3D, original design, no text or logos.
```

**Gemini** · 1:1

```text
Using the attached image as the character reference, create a square 1:1 full-body front view of this Gravion footballer, standing in a neutral A-pose with arms angled 45 degrees away from the body and legs slightly apart, facing the camera straight on. If no image is attached, the character is a stout, broad, heavy alien of smooth dark basalt-like stone with molten amber light glowing through fine cracks, three small pebbles orbiting its shoulders, glowing amber eyes, a deep umber wrap and sash with circular amber emblems, and heavy layered stone boots. The whole body is centered with a generous margin around it, on a plain flat light-grey background under even studio light, with no shadows and nothing else in the frame. Clean painterly stylized 3D rendering. An original design with no lettering or logos.
```

### W1-06 Blinker hero

**ChatGPT** · Portrait

```text
Character concept art: a Blinker footballer, an original alien species. Tall, slender and light-footed, with long limbs and a narrow waist. Its body is semi-translucent faceted crystal: hexagonal facets glowing cyan at the edges and deep violet inside, like a figure carved out of light. A small smooth head with two large luminous ice-blue eyes and a calm, curious expression. Faint after-images of its own silhouette trail behind it, as if it is in two places at once. It wears a flowing violet kit of ribbon-like cloth patterned with small hexagons, and light pointed boots. Pose: mid-stride on tiptoe, one foot flicking a small glowing ball, leaning forward with grace and speed. Background: a softly blurred floating stadium under a cyan and violet nebula. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 2:3

```text
Create a 2:3 portrait of character concept art for a Blinker footballer, an original alien species. The Blinker is tall, slender and light-footed, with long limbs and a narrow waist. Its body is semi-translucent faceted crystal, with hexagonal facets glowing cyan at the edges and deep violet inside, like a figure carved out of light. It has a small smooth head with two large luminous ice-blue eyes and a calm, curious expression. Faint after-images of its own silhouette trail behind it, as if it is in two places at once. It wears a flowing violet kit of ribbon-like cloth patterned with small hexagons, and light pointed boots. It is caught mid-stride on tiptoe, one foot flicking a small glowing ball, leaning forward with grace and speed. Behind it, a floating stadium under a cyan and violet nebula is softly out of focus, as if shot with a long lens. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W1-07 Blinker turnaround sheet

Attach W1-06 when you send this prompt.

**ChatGPT** · Landscape

```text
Character turnaround model sheet for the Blinker footballer (attached image, if any): a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, light pointed boots. Show the same character five times in a row, evenly spaced, at exactly the same scale and standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view, back view. Neutral A-pose: arms angled 45 degrees away from the body, legs slightly apart, hands relaxed. No after-image trails in this sheet. Orthographic, no perspective distortion. Plain flat light-grey background, flat even studio lighting, no cast shadows, no props, no ground plane. Identical proportions and details in every view. Style: clean painterly stylized 3D, original design, no text, labels, arrows or logos.
```

**Gemini** · 21:9

```text
Using the attached image as the character reference, create an ultra-wide 21:9 character turnaround model sheet of this Blinker footballer. If no image is attached, the character is a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, and light pointed boots. Show the same character five times in a row, evenly spaced, at exactly the same scale, standing on one shared ground line: front view, three-quarter front view, side view, three-quarter back view and back view. In every view it holds a neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, hands relaxed, with its body clear and still and no after-image trails. Each view is orthographic with no perspective distortion, and the proportions and details match exactly from view to view. The background is plain flat light grey, lit by even studio light, with the figures standing on nothing and casting no shadows. Clean painterly stylized 3D rendering. An original design; pure artwork with no labels, arrows, lettering or logos.
```

### W1-07b Blinker front view for 3D

**ChatGPT** · Square

```text
Full-body front view of the Blinker footballer (attached image, if any): a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, light pointed boots. Neutral A-pose, arms angled 45 degrees away from the body, legs slightly apart, facing the camera straight on, no after-images. The whole body is centered and fully visible with a margin all around. Plain flat light-grey background, flat even lighting, no shadows, no props. Style: clean painterly stylized 3D, original design, no text or logos.
```

**Gemini** · 1:1

```text
Using the attached image as the character reference, create a square 1:1 full-body front view of this Blinker footballer, standing in a neutral A-pose with arms angled 45 degrees away from the body and legs slightly apart, facing the camera straight on, its body clear and still with no after-image trails. If no image is attached, the character is a tall, slender, light-footed alien with a semi-translucent body of faceted crystal, hexagonal facets glowing cyan at the edges and deep violet inside, a small smooth head with two large luminous ice-blue eyes, a flowing violet kit of ribbon-like cloth patterned with small hexagons, and light pointed boots. The whole body is centered with a generous margin around it, on a plain flat light-grey background under even studio light, with no shadows and nothing else in the frame. Clean painterly stylized 3D rendering. An original design with no lettering or logos.
```

### W1-08 The ball

**ChatGPT** · Square

```text
Prop design sheet: the official ball of Intergalactic Football, a sphere the size of a football. A polished pearl-white shell built from interlocking hexagonal and circular panels (one shape for each species), with a soft star-like core glowing pale cyan-white through the seams between panels. Show the ball three times on one sheet: front view, top view, and in flight with a short luminous trail. Plain dark indigo background, soft studio lighting. Style: painterly stylized 3D, rich color, original design, no text or logos.
```

**Gemini** · 1:1

```text
Create a square 1:1 prop design sheet for the official ball of Intergalactic Football, a sphere the size of a football. Its polished pearl-white shell is built from interlocking hexagonal and circular panels, one shape for each species, and a soft star-like core glows pale cyan-white through the seams between them. Show the ball three times on the sheet: a front view, a top view, and in flight with a short luminous trail. The background is plain dark indigo under soft studio light. Painterly stylized 3D with rich color. An original design; pure artwork with no lettering or logos.
```

### W1-09 Pitch turf texture

Use: tiled across the pitch as a material. I'll repair the seams if it doesn't tile perfectly.

**ChatGPT** · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit, with no shadows and no perspective: alien turf for a cosmic football pitch. Short, soft, moss-like blades in deep teal (#0F4A44) and green-teal, with tiny bioluminescent cyan specks scattered sparsely. Subtle natural variation, with no obvious repeating features, no lines and no grid. The edges must tile seamlessly in both directions. No text.
```

**Gemini** · 1:1

```text
Create a square 1:1 seamless, tileable texture viewed straight down under flat, even light with no shadows or perspective: alien turf for a cosmic football pitch. Short, soft, moss-like blades in deep teal (#0F4A44) and green-teal are sprinkled with sparse, tiny bioluminescent cyan specks. The variation is subtle and natural, and the surface stays uniform from edge to edge, with no lines, grid or standout features. The left edge continues seamlessly into the right and the top into the bottom. Pure texture with no lettering.
```

### W1-10 Storyboard: kick-off to goal

Use: the shots, framing and beats of the first animated sequence. I drive the player movement from a real match played by the rules engine and use this board for the cinematography.

**ChatGPT** · Landscape

```text
A six-panel cinematic storyboard in a 3 by 2 grid for an animated sequence in Intergalactic Football, painted in a loose but polished style. Panel 1: wide shot of the floating stadium at kick-off, stout amber-glowing stone Gravions and slender cyan crystal Blinkers lined up on the glowing grid pitch. Panel 2: a Gravion passes the glowing ball along the turf, leaving a trail of amber light. Panel 3: a Blinker leaps and intercepts, its crystal body flaring cyan. Panel 4: the Blinker bursts into hexagonal shards and reappears a few tiles ahead, after-images fading behind. Panel 5: a low dramatic angle as the Blinker strikes the ball toward the amber goal arch while the Gravion keeper dives. Panel 6: the goal arch erupts into ribbons of cyan light and the crowd's lanterns float up into the sky. Thin dark borders between panels. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text, numbers or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 cinematic storyboard of six panels arranged in a 3 by 2 grid with thin dark borders, painted in a loose but polished style, for an animated sequence in Intergalactic Football. Panel 1 is a wide shot of the floating stadium at kick-off, with stout amber-glowing stone Gravions and slender cyan crystal Blinkers lined up on the glowing grid pitch. In panel 2 a Gravion passes the glowing ball along the turf, leaving a trail of amber light. In panel 3 a Blinker leaps to intercept, its crystal body flaring cyan. In panel 4 the Blinker bursts into hexagonal shards and reappears a few tiles ahead as after-images fade behind it. Panel 5 is a low, dramatic angle as the Blinker strikes the ball toward the amber goal arch while the Gravion keeper dives. In panel 6 the goal arch erupts into ribbons of cyan light and the crowd's lanterns float up into the sky. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

---

## Wave 2: powers, ultimates and moments

Each power is a 4-stage strip so I can time the effect in Blender: build-up, release, peak, fade. All strips use the gameplay camera, a high three-quarter view over the grid pitch.

### W2-01 Gravity Well (Gravion)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, release, peak, fade. Effect: Gravity Well. A stout stone Gravion with molten amber cracks raises one heavy hand; space ripples around it like a pond; concentric rings of amber light warp the turf of the glowing grid pitch; the glowing ball is pulled off the foot of a startled slender cyan crystal Blinker and curves through the air toward the Gravion; the pebbles orbiting the Gravion's shoulders spin faster and faster. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: build-up, release, peak and fade. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Gravity Well. A stout stone Gravion with molten amber cracks raises one heavy hand, and space ripples around it like the surface of a pond. Concentric rings of amber light warp the turf, the glowing ball is pulled off the foot of a startled slender cyan crystal Blinker and curves through the air toward the Gravion, and the pebbles orbiting the Gravion's shoulders spin faster and faster. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-02 Shockwave (Gravion)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, release, peak, fade. Effect: Shockwave. A stout stone Gravion with molten amber cracks stamps one foot on the glowing grid pitch; a ring of molten amber force bursts outward from its feet, lifting glowing dust and blades of teal turf; two slender cyan crystal Blinkers next to it are pushed back one step, their bodies flickering; the ball pops loose from one of them. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: build-up, release, peak and fade. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Shockwave. A stout stone Gravion with molten amber cracks stamps one foot, and a ring of molten amber force bursts outward from its feet, lifting glowing dust and blades of teal turf. Two slender cyan crystal Blinkers beside it are pushed back one step, their bodies flickering, and the ball pops loose from one of them. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-03 Meteor Strike (Gravion)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: wind-up, strike, flight, impact. Effect: Meteor Strike. A stout stone Gravion striker draws back and kicks the glowing ball so hard that it ignites into a small meteor wrapped in molten amber fire and shards of rock. It tears a straight, blazing path across the grid pitch toward a cyan goal arch, passing untouched through the shimmering zones of two crystal Blinker defenders. Only the Blinker keeper stands in the goal, bracing. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: wind-up, strike, flight and impact. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Meteor Strike. A stout stone Gravion striker draws back and kicks the glowing ball so hard that it ignites into a small meteor wrapped in molten amber fire and shards of rock. It tears a straight, blazing path across the pitch toward a cyan goal arch, passing untouched through the shimmering zones of two crystal Blinker defenders, while the Blinker keeper braces alone in the goal. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-04 Blink (Blinker)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, vanish, reappear, fade. Effect: Blink. A slender cyan crystal Blinker carrying the glowing ball dissolves into a swirl of hexagonal crystal shards and reappears three tiles away on the glowing grid pitch in a flash of cyan light. A chain of fading after-images links the two spots, and two stout amber stone Gravions turn their heads too late. Same high three-quarter game camera in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: build-up, vanish, reappear and fade. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Blink. A slender cyan crystal Blinker carrying the glowing ball dissolves into a swirl of hexagonal crystal shards and reappears three tiles away in a flash of cyan light. A chain of fading after-images links the two spots, and two stout amber stone Gravions turn their heads a moment too late. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-05 Phase Pass (Blinker)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: wind-up, release, phasing, catch. Effect: Phase Pass. A slender cyan crystal Blinker passes the ball; the ball turns ghostly and translucent and streams as a cyan wisp straight through two stout amber stone Gravion defenders, leaving hexagonal ripples hanging in the air; a teammate catches it cleanly on the far side. Same high three-quarter game camera over the glowing grid pitch in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: wind-up, release, phasing and catch. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Phase Pass. A slender cyan crystal Blinker passes the ball, which turns ghostly and translucent and streams as a cyan wisp straight through two stout amber stone Gravion defenders, leaving hexagonal ripples hanging in the air, before a teammate catches it cleanly on the far side. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-06 Rift Tackle (Blinker)

**ChatGPT** · Landscape

```text
A horizontal 4-panel effect strip, panels left to right: build-up, rift opens, tackle, rift closes. Effect: Rift Tackle. A slender cyan crystal Blinker defender tears open a thin vertical rift of violet light in the air, steps through it, and emerges right beside a stout amber stone Gravion who is carrying the ball, sliding in to win it. The rift seals behind them in a shower of violet sparks. Same high three-quarter game camera over the glowing grid pitch in every panel. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 effect strip of four equal panels side by side, reading left to right: build-up, rift opens, tackle and rift closes. Every panel uses the same high three-quarter gameplay camera over the glowing grid pitch of the floating stadium. The effect is Rift Tackle. A slender cyan crystal Blinker defender tears open a thin vertical rift of violet light in the air, steps through it and emerges right beside a stout amber stone Gravion carrying the ball, sliding in to win it, as the rift seals behind them in a shower of violet sparks. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-07 Ultimate: Crush

**ChatGPT** · Landscape

```text
Key art for the Gravion ultimate, Crush. The whole Gravion team, stout stone beings with molten amber cracks, raise their arms together in the middle of the floating stadium. The sky darkens and a colossal dome of amber gravity presses down on the pitch. The slender cyan crystal Blinkers move as if wading through deep water: their light trails are slowed and stretched, and dust and pebbles hang frozen in the air. Heavy, epic, slow and beautiful. Wide cinematic shot. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 cinematic key art image of the Gravion ultimate, Crush. In the middle of the floating stadium, the whole Gravion team, stout stone beings with molten amber cracks, raise their arms together. The sky darkens and a colossal dome of amber gravity presses down onto the pitch. The slender cyan crystal Blinkers move as if wading through deep water, their light trails slowed and stretched, while dust and pebbles hang frozen in the air. The moment feels heavy, epic, slow and beautiful. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-08 Ultimate: Fold Space

**ChatGPT** · Landscape

```text
Key art for the Blinker ultimate, Fold Space. The glowing grid pitch folds like a sheet of paper along seams of hexagonal light, curling up toward the nebula sky in a wave. Two slender cyan crystal Blinkers swap places through the fold, each leaving a spiral of after-images. Stout amber stone Gravions stare up in wonder. Wide cinematic shot, playful and impossible. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 cinematic key art image of the Blinker ultimate, Fold Space. The glowing grid pitch folds like a sheet of paper along seams of hexagonal light and curls up toward the nebula sky in a great wave. Two slender cyan crystal Blinkers swap places through the fold, each leaving a spiral of after-images, while stout amber stone Gravions stare up in wonder. The moment feels playful and impossible. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-09 Gravion goal celebration

**ChatGPT** · Landscape

```text
A horizontal 4-panel storyboard of a goal celebration by the Gravions, stout stone beings with molten amber cracks. Panel 1: the scorer plants both feet and the ground glows amber beneath them. Panel 2: teammates gather in a circle and press their heavy hands together. Panel 3: their shoulder pebbles rise and orbit above the group like a small planetary system. Panel 4: a slow, warm pulse of amber light ripples out across the stadium and the crowd's lanterns answer it. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 storyboard of four equal panels side by side, reading left to right, showing a goal celebration by the Gravions, stout stone beings with molten amber cracks. In the first panel the scorer plants both feet and the ground glows amber beneath them. In the second, teammates gather in a circle and press their heavy hands together. In the third, their shoulder pebbles rise and orbit above the group like a small planetary system. In the fourth, a slow, warm pulse of amber light ripples out across the stadium and the crowd's lanterns answer it. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-10 Blinker goal celebration

**ChatGPT** · Landscape

```text
A horizontal 4-panel storyboard of a goal celebration by the Blinkers, slender translucent crystal beings glowing cyan and violet. Panel 1: the scorer spins on tiptoe, leaving a ring of after-images. Panel 2: they blink rapidly across the pitch in a zigzag of cyan flashes. Panel 3: the whole team appears in a hexagon formation, each player in a different place at once. Panel 4: they burst into a fountain of hexagonal light shards that drift up into the nebula like snow falling upward. Style: painterly stylized 3D film stills, dreamlike cosmic wonder, volumetric glow, rich saturated color, original design, no text or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 storyboard of four equal panels side by side, reading left to right, showing a goal celebration by the Blinkers, slender translucent crystal beings glowing cyan and violet. In the first panel the scorer spins on tiptoe, leaving a ring of after-images. In the second, they blink across the pitch in a zigzag of cyan flashes. In the third, the whole team appears in a hexagon formation, each player in two places at once. In the fourth, they burst into a fountain of hexagonal light shards that drift up into the nebula like snow falling upward. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
```

### W2-11 Gravion squad line-up

Use: telling the five roles apart by silhouette. Every squad has the same five roles.

**ChatGPT** · Landscape

```text
Line-up sheet of five Gravion footballers standing side by side, same species, same scale, on one shared ground line, front view, neutral A-pose, plain flat light-grey background, flat even lighting, no shadows. All are stout stone beings with molten amber cracks, glowing amber eyes and deep umber kits, but each role has a clearly different silhouette. Keeper: the largest, with broad stone gloves and a faint amber shield glow around the hands. Anchor: very wide shoulders, the heaviest build. Playmaker: balanced, with a circular amber visor over the eyes. Runner: the leanest, with longer legs. Striker: powerful legs, with one boot glowing molten hot. Style: clean painterly stylized 3D, original design, no text, labels or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 line-up sheet of five Gravion footballers standing side by side at the same scale on one shared ground line, in front view and a neutral A-pose, on a plain flat light-grey background under even studio light with no shadows. All five are stout stone beings with molten amber cracks, glowing amber eyes and deep umber kits, and each role has its own clear silhouette. The keeper is the largest, with broad stone gloves and a faint amber shield glow around the hands. The anchor has very wide shoulders and the heaviest build. The playmaker is balanced, with a circular amber visor over the eyes. The runner is the leanest, with longer legs. The striker has powerful legs and one boot glowing molten hot. Clean painterly stylized 3D rendering. An original design; pure artwork with no labels, arrows, lettering or logos.
```

### W2-12 Blinker squad line-up

**ChatGPT** · Landscape

```text
Line-up sheet of five Blinker footballers standing side by side, same species, same scale, on one shared ground line, front view, neutral A-pose, plain flat light-grey background, flat even lighting, no shadows. All are slender translucent crystal beings with hexagonal facets glowing cyan and violet, luminous ice-blue eyes and flowing violet kits, but each role has a clearly different silhouette. Keeper: the tallest, with long arms and hexagonal light plates on the forearms. Anchor: thicker crystal plating on the chest and shoulders. Playmaker: a halo of small floating hexagons around the head. Runner: the thinnest, with long legs and trailing ribbons. Striker: one leg made of brighter, denser crystal. Style: clean painterly stylized 3D, original design, no text, labels or logos.
```

**Gemini** · 21:9

```text
Create an ultra-wide 21:9 line-up sheet of five Blinker footballers standing side by side at the same scale on one shared ground line, in front view and a neutral A-pose, on a plain flat light-grey background under even studio light with no shadows. All five are slender translucent crystal beings with hexagonal facets glowing cyan and violet, luminous ice-blue eyes and flowing violet kits, and each role has its own clear silhouette. The keeper is the tallest, with long arms and hexagonal light plates on the forearms. The anchor has thicker crystal plating on the chest and shoulders. The playmaker wears a halo of small floating hexagons around the head. The runner is the thinnest, with long legs and trailing ribbons. The striker has one leg of brighter, denser crystal. Clean painterly stylized 3D rendering. An original design; pure artwork with no labels, arrows, lettering or logos.
```

### W2-13 Gravion crest

**ChatGPT** · Square, transparent background

```text
Emblem for the Gravions team: a round crest of carved dark stone with a molten amber core and three small pebbles orbiting it. Bold, simple shapes that read clearly at small sizes, softly painted. Centered on a transparent background. No text or letters.
```

**Gemini** · 1:1

Gemini can't make transparent backgrounds, so this version asks for flat white. I'll cut the crest out.

```text
Create a square 1:1 emblem for the Gravions team: a round crest of carved dark stone with a molten amber core and three small pebbles orbiting it, built from bold, simple shapes that read clearly at small sizes, softly painted. The crest sits centered on a plain flat pure-white background with nothing else in the frame. An original design; pure artwork with no lettering.
```

### W2-14 Blinker crest

**ChatGPT** · Square, transparent background

```text
Emblem for the Blinkers team: a hexagonal crest of cyan and violet crystal with a fast motif of two overlapping after-image outlines. Bold, simple shapes that read clearly at small sizes, softly painted. Centered on a transparent background. No text or letters.
```

**Gemini** · 1:1

Gemini can't make transparent backgrounds, so this version asks for flat white. I'll cut the crest out.

```text
Create a square 1:1 emblem for the Blinkers team: a hexagonal crest of cyan and violet crystal with a motif of two overlapping after-image outlines that suggests speed, built from bold, simple shapes that read clearly at small sizes, softly painted. The crest sits centered on a plain flat pure-white background with nothing else in the frame. An original design; pure artwork with no lettering.
```

---

## Wave 3: materials and set pieces

### W3-01 Gravion stone skin

**ChatGPT** · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit with no shadows or perspective: smooth dark basalt stone like polished river rock, with fine branching cracks glowing molten amber (#FFB02E) and orange (#E07A10) from within. The cracks are sparse and elegant, not chaotic. The edges must tile seamlessly in both directions. No text.
```

**Gemini** · 1:1

```text
Create a square 1:1 seamless, tileable texture viewed straight down under flat, even light with no shadows or perspective: smooth dark basalt stone like polished river rock, with fine branching cracks glowing molten amber (#FFB02E) and orange (#E07A10) from within. The cracks are sparse and elegant and spread evenly from edge to edge. The left edge continues seamlessly into the right and the top into the bottom. Pure texture with no lettering.
```

### W3-02 Blinker crystal

**ChatGPT** · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit: translucent faceted crystal made of irregular hexagonal facets, with cyan (#41E3FF) light along the facet edges and deep violet (#6A4DFF) in the depths, plus faint internal sparkles. The edges must tile seamlessly in both directions. No text.
```

**Gemini** · 1:1

```text
Create a square 1:1 seamless, tileable texture viewed straight down under flat, even light with no shadows or perspective: translucent faceted crystal made of irregular hexagonal facets, with cyan (#41E3FF) light along the facet edges, deep violet (#6A4DFF) in the depths and faint sparkles inside, spread evenly from edge to edge. The left edge continues seamlessly into the right and the top into the bottom. Pure texture with no lettering.
```

### W3-03 Stadium stone walkway

**ChatGPT** · Square

```text
Seamless tileable texture, viewed straight down, flat and evenly lit: ancient dark stone paving for a floating stadium, made of large rounded slabs with thin seams of soft teal moss and an occasional tiny cyan bioluminescent flower. The edges must tile seamlessly in both directions. No text.
```

**Gemini** · 1:1

```text
Create a square 1:1 seamless, tileable texture viewed straight down under flat, even light with no shadows or perspective: ancient dark stone paving for a floating stadium, made of large rounded slabs with thin seams of soft teal moss and an occasional tiny cyan bioluminescent flower, spread evenly from edge to edge. The left edge continues seamlessly into the right and the top into the bottom. Pure texture with no lettering.
```

### W3-04 Goal arch design

**ChatGPT** · Landscape

```text
Prop design sheet: the goal of Intergalactic Football. An arch three tiles wide made of two curved pillars, part dark stone and part crystal, joined at the top, with a shimmering curtain of light hanging between them as the net. Show an amber version and a cyan version, each in front view and side view, four drawings evenly spaced on a plain dark indigo background with soft studio lighting. Style: painterly stylized 3D, rich saturated color, original design, no text, labels or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 prop design sheet for the goal of Intergalactic Football: an arch three tiles wide made of two curved pillars, part dark stone and part crystal, joined at the top, with a shimmering curtain of light hanging between them as the net. Show an amber version and a cyan version, each in front view and side view, as four drawings evenly spaced across a plain dark indigo background under soft studio light. Painterly stylized 3D with rich saturated color. An original design; pure artwork with no labels, lettering or logos.
```

### W3-05 Crowd terraces close-up

**ChatGPT** · Landscape

```text
A close view of one floating crowd terrace at the edge of the Intergalactic Football stadium: a curved slab of stone drifting in space, crowded with many different small alien species of all shapes and colors holding glowing lanterns, banners of pure colored light with no writing, joyful and strange, lit warmly by the glow of the pitch below. Style: painterly stylized 3D film still, dreamlike cosmic wonder, volumetric glow and rim light, rich saturated color, original design, no text or logos.
```

**Gemini** · 16:9

```text
Create a wide 16:9 close view of one floating crowd terrace at the edge of the Intergalactic Football stadium: a curved slab of stone drifting in space, packed with many different small alien species of every shape and color holding glowing lanterns and waving banners made of pure colored light with no writing on them, joyful and strange, lit warmly from below by the glow of the pitch. Rendered as a painterly, stylized 3D frame from a hand-crafted animated film, with dreamlike cosmic wonder, soft volumetric glow, strong rim light and rich saturated color. An original design; pure artwork with no lettering or logos.
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
