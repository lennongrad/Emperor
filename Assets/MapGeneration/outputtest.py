import drawsvg as draw
import pyclipper
import itertools
from PIL import Image, ImageFilter

provinces = {}
bodies = {}
borders = {}

with open("color_id.csv", "r") as f:
    for line in f:
        R, G, B, id = line.strip().split(",")
        color = (int(R), int(G), int(B))
        
        provinces[int(id)] = {
            "color": color,
            "id": id,
            "is-ocean": (color[1] >= 200 and color[2] >= 200 and color[0] <= 125),
            "hex-color": '#%02x%02x%02x' % color, 
            "borders": {},
            "bodies": [],
        }
     
with open("bodies_points.csv", "r") as f:
    for line in f:
        split_line = line.strip().split(",")
        id = int(split_line[0])
        
        points = []
        coords = [int(x) for x in split_line[1:]]
        for coord_index in range(0, len(coords), 2):
            points.append((coords[coord_index], coords[coord_index+1]))
        
        bodies[id] = points

with open("province_bodies.csv", "r") as f:
    for line in f:
        split_line = line.strip().split(",")
        id = int(split_line[0])
        
        loaded_bodies = [int(x) for x in split_line[1:]]
        provinces[id]["bodies"] = loaded_bodies

with open("provinces_borders.csv", "r") as f:
    for line in f:
        split_line = line.strip().split(",")
        id = int(split_line[0])
        other_id = int(split_line[1])
        
        loaded_borders = [int(x) for x in split_line[2:]]
        provinces[id]["borders"][other_id] = loaded_borders

with open("borders_points.csv", "r") as f:
    for line in f:
        split_line = line.strip().split(",")
        id = int(split_line[0])
        
        points = []
        coords = [int(x) for x in split_line[1:]]
        for coord_index in range(0, len(coords), 2):
            points.append((coords[coord_index], coords[coord_index+1]))
        
        borders[id] = points

empty_provinces = [province for province in provinces if len(provinces[province]["borders"]) == 0]   
for province in empty_provinces:
    del provinces[province]

width = 0
height = 0
for id, border in borders.items():
    for point in border:
        if point[0] > width:
            width = point[0]
        if point[1] > height:
            height = point[1]

###############
# DRAWING    
###############

scale = 2
d_pixel = draw.Drawing(width, height, origin=(0,0))
d_pixel.set_pixel_scale(scale)

d = draw.Drawing((width+1), (height+1), origin=(0,0))
d.set_pixel_scale(4)

for id, province in sorted(provinces.items(), key=lambda x: not x[1]["is-ocean"]):    
    for body_id in province["bodies"]:
        body = bodies[body_id]
        
        
        color = (int(body_id / 256), body_id % 256, 0)
        #color = province["hex-color"]
        d_pixel.append(draw.Lines(*list(itertools.chain(*body)),
            close=False, fill='#%02x%02x%02x' % color, stroke='#%02x%02x%02x' % color, stroke_width=0))
            
    
    for other_province, province_borders in province["borders"].items():
        other_color = (255,255,255) if other_province == -1 else provinces[other_province]["color"]
        color = (
            (province["color"][0] + other_color[0]) // 3,
            (province["color"][1] + other_color[1]) // 3,
            (province["color"][2] + other_color[2]) // 3,
        )
        color_hex = '#%02x%02x%02x' % color
        
        # if True:
            # for border_id in province_borders:
                # border = borders[border_id]
                # for pixel in border:
                    # d_pixel.append(draw.Rectangle(pixel[0], pixel[1], 1, 1, fill=color_hex))
        
        if False:
            # for border_id in province_borders:
                # border = borders[border_id]
                # d.append(draw.Lines(*list(itertools.chain(*border)),
                    # close=False, fill="none", stroke=color_hex, stroke_width=2))
            for border_id in province_borders:
                border = borders[border_id]
                d_pixel.append(draw.Lines(*list(itertools.chain(*border)),
                    close=False, fill="none", stroke='#%02x%02x%02x' % color, stroke_width=0.1))

#d.save_svg('example.svg')

final_filename = "mapping.png"
d_pixel.save_png(final_filename)


image = Image.open(final_filename)
aliased = image.filter(ImageFilter.ModeFilter(5))
aliased.save(final_filename)