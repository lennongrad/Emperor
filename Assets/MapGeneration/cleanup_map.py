import random
from PIL import Image

image = Image.open("output_map.bmp", "r").convert("RGB")
width = image.size[0]
height = image.size[1]
pixel_array = list(image.getdata())


adjusted_pixels = []

def get_color(point, pixels):
    if 0 <= point[0] < width and 0 <= point[1] < height:
        return pixels[point[1]*width + point[0]]
    return None

def most_common(lst):
    return max(set(lst), key=lst.count)

for y in range(height):
    for x in range(width):
        point = (x,y)
        
        adjacent_colors = []
        
        for zy in range(-2, 3):
            for zx in range(-2, 3):
                adjacent_pixel = get_color((x + zx, y + zy), pixel_array)
                
                if adjacent_pixel != None:
                    adjacent_colors.append(adjacent_pixel)
        
        pixel_color = get_color(point, pixel_array)
        if pixel_color == None:
            print(point)
        elif adjacent_colors.count(pixel_color) >= 4 or most_common(adjacent_colors) == None:
            adjusted_pixels.append(pixel_color)
        else:
            adjusted_pixels.append(most_common(adjacent_colors))


new_image = Image.new(image.mode, image.size)
new_image.putdata(adjusted_pixels)
new_image.save("clean_map_test.bmp")
new_image.show()