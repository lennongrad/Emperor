import random
from PIL import Image

image = Image.open("lilliemap_4.bmp", "r").convert("RGB")
width = image.size[0]
height = image.size[1]
pixel_array = list(image.getdata())

coastal_land_points_desired = 1200
land_points_desired = 4000
coastal_sea_points_desired = 250
sea_points_desired = 250
starter_threshold = 100
min_threshold = 20

def get_color(point, pixels):
    if 0 < point[0] < width and 0 < point[1] < height:
        return pixels[point[1]*width + point[0]]
    return None

land_points = []
coastal_land_points = []
sea_points = []
coastal_sea_points = []

print("Identify points")
for x in range(width):
    for y in range(height):
        point = (x,y)
        
        adjacent_pixels = [
            (point[0] + 0, point[1] + 1),
            (point[0] + 1, point[1] + 0),
            (point[0] + 0, point[1] - 1),
            (point[0] - 1, point[1] + 0)
        ]
        
        if get_color(point, pixel_array) == (2,2,2):
            land_points.append(point)
              
            if any(get_color(pixel, pixel_array) != (2,2,2) for pixel in adjacent_pixels):
                coastal_land_points.append(point)
        elif get_color(point, pixel_array) == (254,254,254):
            sea_points.append(point)
              
            if any(get_color(pixel, pixel_array) != (254,254,254) for pixel in adjacent_pixels):
                coastal_sea_points.append(point)


chosen_points = []
sea_chosen_points = []

def validate_point(point, points, threshold=10):
    if point in points:
        return False, False
    
    for other_point in points:
        if abs(point[0] - other_point[0]) + abs(point[1] - other_point[1]) < threshold:
            return False, True
    
    return True, False

print("Choose coastal points")
for i in range(coastal_land_points_desired):
    point = random.choice(coastal_land_points)
    
    attempts = 0
    v1, v2 = validate_point(point,chosen_points,threshold=starter_threshold-attempts)
    while not v1:
        point = random.choice(coastal_land_points)
        v1, v2 = validate_point(point,chosen_points,threshold=max(min_threshold, starter_threshold-attempts))
        if v2: 
            attempts += 1
    
    chosen_points.append(point)

print("Choose inland points")
for i in range(land_points_desired):
    point = random.choice(land_points)
    
    attempts = 0
    v1, v2 = validate_point(point,chosen_points,threshold=starter_threshold-attempts)
    while not v1:
        point = random.choice(land_points)
        v1, v2 = validate_point(point,chosen_points,threshold=max(min_threshold, starter_threshold-attempts))
        if v2: 
            attempts += 1
            if attempts > (starter_threshold - min_threshold) + 20:
                print(attempts)
    
    chosen_points.append(point)
    
print("Choose coastal sea points")
for i in range(coastal_sea_points_desired):
    point = random.choice(coastal_sea_points)
    
    attempts = 0
    v1, v2 = validate_point(point,sea_chosen_points,threshold=starter_threshold-attempts)
    while not v1:
        point = random.choice(coastal_sea_points)
        v1, v2 = validate_point(point,sea_chosen_points,threshold=max(min_threshold, starter_threshold-attempts))
        if v2: 
            attempts += 1
    
    sea_chosen_points.append(point)

print("Choose sea points")
for i in range(sea_points_desired):
    point = random.choice(sea_points)
    
    attempts = 0
    v1, v2 = validate_point(point,sea_chosen_points,threshold=starter_threshold-attempts)
    while not v1:
        point = random.choice(sea_points)
        v1, v2 = validate_point(point,sea_chosen_points,threshold=max(min_threshold, starter_threshold-attempts))
        if v2: 
            attempts += 1
            if attempts > (starter_threshold - min_threshold) + 20:
                print(attempts)
    
    sea_chosen_points.append(point)

adjusted_pixels = pixel_array.copy()

colors = []
land_colors = []
sea_colors = []
land_point_queue = []
sea_point_queue = []
visited_points = set()

def get_random_color(is_sea=False):
    if is_sea:
        return (random.randint(3, 50), random.randint(200, 253), random.randint(220, 253))
    return (random.randint(3, 50), random.randint(150, 253), random.randint(3, 150))

print("Choose point colors.")
for point in chosen_points:
    color = get_random_color()
    while color in colors:
        color = get_random_color()
    colors.append(color)
    land_colors.append(color)
    land_point_queue.append((point, color))

for point in sea_chosen_points:
    color = get_random_color(True)
    while color in colors:
        color = get_random_color(True)
    colors.append(color)
    sea_colors.append(color)
    sea_point_queue.append((point, color))

print("Fill in continents")
for q, c in [(land_point_queue, (2,2,2)), (sea_point_queue, (254, 254, 254))]:
    while len(q) > 0:
        current_point, color = q.pop(0)
        
        adjacent_pixels = [
            (current_point[0] + 0, current_point[1] + 1),
            (current_point[0] + 1, current_point[1] + 0),
            (current_point[0] + 0, current_point[1] - 1),
            (current_point[0] - 1, current_point[1] + 0)
        ]
        
        for adjacent_pixel in adjacent_pixels:
            if get_color(adjacent_pixel, adjusted_pixels) == c:
                n = random.randint(0,len(q)+1)
                if c == (254,254,254):
                    n = -1
                q.insert(n, (adjacent_pixel, color))
                adjusted_pixels[adjacent_pixel[1]*width + adjacent_pixel[0]] = color

print("Fill in islands.")

for pixel_x in range(0, width):
    for pixel_y in range(0, height):        
        pixel_color = adjusted_pixels[pixel_y * width + pixel_x] 
        
        if pixel_color == (0,0,0) or pixel_color == (255,255,255):
            new_color = get_random_color(pixel_color == (255,255,255))
            while new_color in colors:
                new_color = get_random_color(pixel_color == (255,255,255))
            colors.append(new_color)
            land_colors.append(new_color)
            
            pixel_queue = [(pixel_x, pixel_y)]
            adjusted_pixels[pixel_y * width + pixel_x]  = new_color
            
            while len(pixel_queue) > 0:
                current_pixel = pixel_queue.pop(0)
                
                adjacent_pixels = [
                    (current_pixel[0] + 0, current_pixel[1] + 1),
                    (current_pixel[0] + 1, current_pixel[1] + 0),
                    (current_pixel[0] + 0, current_pixel[1] - 1),
                    (current_pixel[0] - 1, current_pixel[1] + 0)
                ]
                
                for adjacent_pixel in adjacent_pixels:       
                    if get_color(adjacent_pixel, adjusted_pixels) == pixel_color:
                        pixel_queue.append(adjacent_pixel)
                        adjusted_pixels[(adjacent_pixel[1]) * width + adjacent_pixel[0]] = new_color

print("Identify color IDs.")
with open("color_id_lillie.csv", "w") as f:
    id = 0
    for color in land_colors:
        f.write(f"{color[0]},{color[1]},{color[2]},{id},True\n")
        id += 1
    for index, color in enumerate(sea_colors):
        f.write(f"{color[0]},{color[1]},{color[2]},{id},False")
        id += 1
        if index != len(sea_colors) - 1:
            f.write("\n")

new_image = Image.new(image.mode, image.size)
new_image.putdata(adjusted_pixels)
new_image.save("lilliemap_4_test.bmp")
new_image.show()