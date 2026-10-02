import math
import os

from PIL import Image, ImageDraw, ImageFilter, ImageFont

FULL_NAME = "JOSHUA CHUKWUEBUKA MOSES"
SIZE = 1024
RED = (255, 70, 60)
AMBER = (255, 190, 56)
ROOT = os.path.join(os.path.dirname(__file__), "..", "Assets", "HoldfastAR")
OUT = os.path.join(ROOT, "Resources", "Textures", "PlaneTrackerName.png")
FONT = os.path.join(ROOT, "Resources", "Fonts", "KenneyFuture.ttf")


def hex_points(cx, cy, r):
    return [(cx + r * math.cos(math.radians(60 * i + 30)), cy + r * math.sin(math.radians(60 * i + 30)))
            for i in range(6)]


def main():
    img = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([0, 0, SIZE, SIZE], fill=(14, 6, 26, 70))

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
            draw.polygon(hex_points(cx, cy, r * 0.96), outline=RED + (80,))

    draw.rectangle([0, 0, SIZE - 1, SIZE - 1], outline=RED + (170,), width=6)

    banner = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    bd = ImageDraw.Draw(banner)
    bd.rounded_rectangle([60, 330, SIZE - 60, 700], radius=36, fill=(12, 4, 22, 190), outline=RED + (255,), width=6)

    font_big = ImageFont.truetype(FONT, 78)
    font_small = ImageFont.truetype(FONT, 36)
    y = 372
    for line in FULL_NAME.split(" "):
        tw = bd.textlength(line, font=font_big)
        bd.text(((SIZE - tw) / 2, y), line, font=font_big, fill=(255, 255, 255, 255))
        y += 104
    tag = "HOLDFAST AR // LANDING ZONE"
    tw = bd.textlength(tag, font=font_small)
    bd.text(((SIZE - tw) / 2, 730), tag, font=font_small, fill=AMBER + (235,))

    glow = banner.filter(ImageFilter.GaussianBlur(8))
    img = Image.alpha_composite(img, glow)
    img = Image.alpha_composite(img, banner)

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    img.save(OUT)
    print("wrote", os.path.normpath(OUT))


if __name__ == "__main__":
    main()
