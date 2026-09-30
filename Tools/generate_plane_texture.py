"""
Generates the custom AR plane-tracker texture for Holdfast AR.
The tile shows a hex-grid scanner pattern with the developer's full name,
so every detected plane is visibly branded "JOSHUA CHUKWUEBUKA MOSES".

Run:  python Tools/generate_plane_texture.py
Output: Assets/HoldfastAR/Resources/Textures/PlaneTrackerName.png (1024x1024, RGBA, tileable)
"""
import math
import os

from PIL import Image, ImageDraw, ImageFilter, ImageFont

FULL_NAME = "JOSHUA CHUKWUEBUKA MOSES"
SIZE = 1024
CYAN = (40, 230, 255)
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "HoldfastAR", "Resources", "Textures",
                   "PlaneTrackerName.png")
FONT = "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"


def hex_points(cx, cy, r):
    return [(cx + r * math.cos(math.radians(60 * i + 30)), cy + r * math.sin(math.radians(60 * i + 30)))
            for i in range(6)]


def main():
    img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Soft translucent fill so the plane reads as a surface.
    draw.rectangle([0, 0, SIZE, SIZE], fill=(10, 60, 80, 55))

    # Tileable hex grid: spacing chosen so the pattern wraps at the texture edges.
    r = 64
    w = math.sqrt(3) * r
    cols = round(SIZE / w)
    w = SIZE / cols
    r = w / math.sqrt(3)
    h = 1.5 * r
    rows = round(SIZE / h)
    for row in range(-1, rows + 2):
        for col in range(-1, cols + 2):
            cx = col * w + (w / 2 if row % 2 else 0)
            cy = row * (SIZE / rows)
            draw.polygon(hex_points(cx, cy, r * 0.96), outline=CYAN + (90,))

    # Tile border (shows the tiling as a subtle scanner grid).
    draw.rectangle([0, 0, SIZE - 1, SIZE - 1], outline=CYAN + (160,), width=6)

    # Name banner in the centre of the tile.
    banner = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    bd = ImageDraw.Draw(banner)
    bd.rounded_rectangle([70, 340, SIZE - 70, 690], radius=40, fill=(0, 20, 30, 170), outline=CYAN + (255,), width=6)

    lines = FULL_NAME.split(" ")
    font_big = ImageFont.truetype(FONT, 86)
    font_small = ImageFont.truetype(FONT, 40)
    y = 385
    for line in lines:
        tw = bd.textlength(line, font=font_big)
        bd.text(((SIZE - tw) / 2, y), line, font=font_big, fill=(255, 255, 255, 255))
        y += 96
    tag = "HOLDFAST AR  //  PLANE DETECTED"
    tw = bd.textlength(tag, font=font_small)
    bd.text(((SIZE - tw) / 2, 720), tag, font=font_small, fill=CYAN + (230,))

    glow = banner.filter(ImageFilter.GaussianBlur(8))
    img = Image.alpha_composite(img, glow)
    img = Image.alpha_composite(img, banner)

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    img.save(OUT)
    print("wrote", os.path.normpath(OUT))


if __name__ == "__main__":
    main()
