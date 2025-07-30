import random
from PIL import Image

image = Image.open("provinces_final.bmp", "r").convert("RGB")
width = image.size[0]
height = image.size[1]
pixel_array = list(image.getdata())

adjusted_pixels = []
for pixel in pixel_array:
    if pixel[2] >= 200:
        adjusted_pixels.append((254,254,254))
    else:
        adjusted_pixels.append(pixel)

new_image = Image.new(image.mode, image.size)
new_image.putdata(adjusted_pixels)
new_image.save("provinces_final_2.bmp")
new_image.show()