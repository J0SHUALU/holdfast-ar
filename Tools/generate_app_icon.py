"""
Generates the Holdfast AR app icon (original artwork).
A glowing hexagonal "holdfast" shield with a crosshair over a deep-teal gradient,
matching the hex pattern of the custom plane tracker.

Run:  python Tools/generate_app_icon.py
Output: Assets/HoldfastAR/Art/AppIcon.png (1024x1024, opaque RGB - required by iOS)
"""
import math
import os

from PIL import Image, ImageDraw, ImageFilter, ImageFont

S = 2048  # draw at 2x, downscale for smooth edges
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "HoldfastAR", "Art", "AppIcon.png")
FONT = "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"
CYAN = (40, 230, 255)
C = S / 2


def hexagon(r, rot=30, cx=C, cy=C):
    return [(cx + r * math.cos(math.radians(60 * i + rot)), cy + r * math.sin(math.radians(60 * i + rot)))
            for i in range(6)]


def main():
    # Background: radial teal-to-navy gradient.
    bg = Image.new("RGB", (S, S))
    px = bg.load()
    for y in range(S):
        for x in range(S):
            d = math.hypot(x - C, y - C * 0.9) / (S * 0.75)
            t = min(1.0, d)
            px[x, y] = (int(8 + 6 * (1 - t)), int(40 * (1 - t) + 14), int(60 * (1 - t) + 26))
    img = bg.convert("RGBA")

    # Faint hex grid in the background (echoes the plane tracker texture).
    grid = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    g = ImageDraw.Draw(grid)
    r = 150
    w = math.sqrt(3) * r
    for row in range(-1, 10):
        for col in range(-1, 10):
            cx = col * w + (w / 2 if row % 2 else 0)
            cy = row * 1.5 * r
            g.polygon(hexagon(r * 0.95, 30, cx, cy), outline=CYAN + (28,), width=5)
    img = Image.alpha_composite(img, grid)

    # Glowing shield hexagon.
    glow = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.polygon(hexagon(720), outline=CYAN + (255,), width=70)
    img = Image.alpha_composite(img, glow.filter(ImageFilter.GaussianBlur(45)))

    shield = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    sd = ImageDraw.Draw(shield)
    sd.polygon(hexagon(720), fill=(6, 28, 38, 235))
    sd.polygon(hexagon(720), outline=CYAN + (255,), width=46)
    sd.polygon(hexagon(615), outline=CYAN + (110,), width=12)

    # Crosshair.
    cy = C - 110
    sd.ellipse([C - 230, cy - 230, C + 230, cy + 230], outline=(255, 255, 255, 255), width=34)
    for dx, dy in ((0, -1), (0, 1), (-1, 0), (1, 0)):
        x0, y0 = C + dx * 160, cy + dy * 160
        x1, y1 = C + dx * 320, cy + dy * 320
        sd.line([x0, y0, x1, y1], fill=(255, 255, 255, 255), width=34)
    sd.ellipse([C - 46, cy - 46, C + 46, cy + 46], fill=(255, 70, 60, 255))

    # "AR" tag.
    font = ImageFont.truetype(FONT, 190)
    tw = sd.textlength("AR", font=font)
    sd.text((C - tw / 2, C + 250), "AR", font=font, fill=CYAN + (255,))

    img = Image.alpha_composite(img, shield)
    img = img.convert("RGB").resize((1024, 1024), Image.LANCZOS)
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    img.save(OUT)
    print("wrote", os.path.normpath(OUT))


if __name__ == "__main__":
    main()
