import random
from PIL import Image

image = Image.open("provinces_final.bmp", "r").convert("RGB")
width = image.size[0]
height = image.size[1]
pixel_array = list(image.getdata())

seen_colors = set()
land_colors = []
sea_colors = []

wastelands = [
    (64,64,64),
    (90,90,90),
    (110,110,110),
    (140,140,140)
]

for wasteland in wastelands:
    seen_colors.add(wasteland)

adjusted_pixels = []
missed = 0
for index, pixel in enumerate(pixel_array):
    if pixel not in seen_colors:
        seen_colors.add(pixel)
        if pixel[2] >= 200:
            sea_colors.append(pixel)
        else:
            land_colors.append(pixel)
    
    if pixel in [(0,0,0), (255,255,255), (2,2,2), (254,254,254)]:
        adjusted_pixels.append((255,255,255))
        missed += 1
    else:
        adjusted_pixels.append((0,0,0))

land_colors.sort(key=lambda x: x[0]*255*255 + x[1]*255 + x[2])
sea_colors.sort(key=lambda x: x[0]*255*255 + x[1]*255 + x[2])

with open("color_id_lillie.csv", "w") as f:
    id = 0
    
    for color in wastelands:
        f.write(f"{color[0]},{color[1]},{color[2]},{id},True\n")
        id += 1    
    for color in land_colors:
        f.write(f"{color[0]},{color[1]},{color[2]},{id},True\n")
        id += 1
    for index, color in enumerate(sea_colors):
        f.write(f"{color[0]},{color[1]},{color[2]},{id},False")
        id += 1
        if index != len(sea_colors) - 1:
            f.write("\n")

if missed > 0:
    new_image = Image.new(image.mode, image.size)
    new_image.putdata(adjusted_pixels)
    new_image.show()