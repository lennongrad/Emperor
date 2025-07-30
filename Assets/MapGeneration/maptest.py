from PIL import Image
import itertools
import math
import random
import numpy as np
from sklearn.neighbors import NearestNeighbors
import networkx as nx

image = Image.open("lilliemap_4_test.bmp", "r").convert("RGB")
width = image.size[0]
height = image.size[1]
pixel_array = list(image.getdata())
colors = list(set(pixel_array))
provinces = []
direction_conversion = [6, 6, 0, 0, 2, 2, 4, 4]
provinces_by_color = {}

def in_bounds(tuple):
    return (0 <= tuple[0] < width) and (0 <= tuple[1] < height)

def get_distance(t1, t2):
    return (t1[0] - t2[0]) ** 2 + (t1[1] - t2[1]) ** 2
    
def rotate(l, n=1):
    return l[-n:] + l[:-n]

split_pixel_array = [(0,0,0,0) for x in range(len(pixel_array))]
seen_pixels = set()
seen_colors = {}
global_id = 1

for pixel_x in range(0, width):
    for pixel_y in range(0, height):
        if (pixel_x, pixel_y) not in seen_pixels:
            pixel_color = pixel_array[(pixel_y) * width + pixel_x]
            
            contiguous_pixels = set([(pixel_x, pixel_y)])
            pixel_queue = [(pixel_x, pixel_y)]
            
            while len(pixel_queue) > 0:
                current_pixel = pixel_queue.pop()
                
                seen_pixels.add(current_pixel)
                
                for direction in [(1,0), (0,1), (-1,0), (0,-1)]:
                    new_pos = (current_pixel[0] + direction[0], current_pixel[1] + direction[1])
                    
                    if in_bounds(new_pos) and new_pos not in contiguous_pixels and new_pos not in seen_pixels and pixel_array[new_pos[1] * width + new_pos[0]] == pixel_color:
                        pixel_queue.append(new_pos)
                        contiguous_pixels.add(new_pos)
            
            if pixel_color not in seen_colors:
                seen_colors[pixel_color] = 0
            
            new_color = (pixel_color[0], pixel_color[1], pixel_color[2], seen_colors[pixel_color])
            for pixel in contiguous_pixels:
                split_pixel_array[pixel[1] * width + pixel[0]] = new_color
            
            provinces_by_color[new_color] = {
                "color": pixel_color,
                "id": new_color[3],
                "id_string": f"{pixel_color[0]},{pixel_color[1]},{pixel_color[2]},{new_color[3]}",
                "global_id": global_id,
                "is-ocean": pixel_color[2] >= 200,
                "color-hex": '#%02x%02x%02x' % pixel_color, 
                "base-points": {},
                "borders": {},
                "points": [], 
                "main-border": [],
                "area": len(contiguous_pixels)
            }
            
            global_id += 1
            seen_colors[pixel_color] += 1

for pixel_x in range(1, width-1):
    for pixel_y in range(1, height-1):
        adjacent_colors = sorted([
            split_pixel_array[(pixel_y+1) * width + (pixel_x)],
            split_pixel_array[(pixel_y-1) * width + (pixel_x)],
            split_pixel_array[(pixel_y) * width + (pixel_x+1)],
            split_pixel_array[(pixel_y) * width + (pixel_x-1)]
        ])
        
        for color in adjacent_colors:
            if adjacent_colors.count(color) >= 3:
                split_pixel_array[pixel_y * width + pixel_x] = color

for corner_x in range(width + 1):
    for corner_y in range(height + 1):
        main_pos = (corner_x, corner_y)
        upleft_pos = (corner_x - 1, corner_y - 1)
        upright_pos = (corner_x, corner_y - 1)
        downleft_pos = (corner_x - 1, corner_y)
        downright_pos = (corner_x, corner_y)
        
        upleft_pixel = None
        upright_pixel = None
        downleft_pixel = None
        downright_pixel = None
        
        if in_bounds(upleft_pos):
            upleft_pixel = split_pixel_array[upleft_pos[1] * width + upleft_pos[0]]
        if in_bounds(upright_pos):
            upright_pixel = split_pixel_array[upright_pos[1] * width + upright_pos[0]]
        if in_bounds(downleft_pos):
            downleft_pixel = split_pixel_array[downleft_pos[1] * width + downleft_pos[0]]
        if in_bounds(downright_pos):
            downright_pixel = split_pixel_array[downright_pos[1] * width + downright_pos[0]]
        
        pixels = [upleft_pixel, upright_pixel, downleft_pixel, downright_pixel]
        for pixel in pixels:
            if pixel != None:
                for other_pixel in pixels:
                    if other_pixel != pixel and (other_pixel == None or pixel not in provinces_by_color[other_pixel]["base-points"]):
                        if other_pixel not in provinces_by_color[pixel]["base-points"]:
                            provinces_by_color[pixel]["base-points"][other_pixel] = set()
                        provinces_by_color[pixel]["base-points"][other_pixel].add(main_pos)

def get_clockwise(position, rotation):
    match rotation:
        case 0: return (position[0], position[1] + 1)
        case 1: return (position[0] - 1, position[1] + 1)
        case 2: return (position[0] - 1, position[1])
        case 3: return (position[0] - 1, position[1] - 1)
        case 4: return (position[0], position[1] - 1)
        case 5: return (position[0] + 1, position[1] - 1)
        case 6: return (position[0] + 1, position[1])
        case 7: return (position[0] + 1, position[1] + 1)

global_borders = []
for p_c in provinces_by_color:
    province = provinces_by_color[p_c]
    for color, border_set in province["base-points"].items():
        adjusted_borders = set()
        
        for pixel in border_set:
            br_rects = [pixel, (pixel[0], pixel[1]+1), (pixel[0]+1, pixel[1]), (pixel[0]+1, pixel[1]+1)]
            bl_rects = [pixel, (pixel[0], pixel[1]+1), (pixel[0]-1, pixel[1]), (pixel[0]-1, pixel[1]+1)]
            tr_rects = [pixel, (pixel[0], pixel[1]-1), (pixel[0]+1, pixel[1]), (pixel[0]+1, pixel[1]-1)]
            tl_rects = [pixel, (pixel[0], pixel[1]-1), (pixel[0]-1, pixel[1]), (pixel[0]-1, pixel[1]-1)]
            if (all(x in border_set for x in br_rects) or all(x in border_set for x in bl_rects) or all(x in border_set for x in tr_rects) or all(x in border_set for x in tl_rects)):
                adjacent = [(pixel[0]-1,pixel[1]), (pixel[0]+1,pixel[1]), (pixel[0],pixel[1]-1), (pixel[0],pixel[1]+1)]
                if len(list(x for x in adjacent if x in border_set)) >= 3:
                    adjusted_borders.add(pixel)
            else:
                adjusted_borders.add(pixel)
        remaining_pixels = list(adjusted_borders)
        
        while len(remaining_pixels) > 2:
            points = np.array(list(remaining_pixels))

            clf = NearestNeighbors(n_neighbors=2).fit(points)
            G = clf.kneighbors_graph()

            T = nx.from_scipy_sparse_array(G)

            paths = [list(nx.dfs_preorder_nodes(T, i)) for i in range(len(points))]
            biggest_path_size = (max(len(x) for x in paths))
            
            mindist = np.inf
            minidx = 0

            for i in range(len(points)):
                p = paths[i]
                if len(p) >= biggest_path_size:
                    ordered = points[p]
                    cost = (((ordered[:-1] - ordered[1:])**2).sum(1)).sum()
                    if cost < mindist:
                        mindist = cost
                        minidx = i

            opt_order = paths[minidx]
            
            pixels = list((int(x[0]), int(x[1])) for x in list(map(tuple, points[opt_order])))
            
            for pixel in pixels:
                remaining_pixels.remove(pixel)

            global_borders.append(pixels)
            border_id = len(global_borders) - 1
            
            if color not in province["borders"]:
                province["borders"][color] = []
            province["borders"][color].append(border_id)
            if color != None:
                if p_c not in provinces_by_color[color]["borders"]:
                    provinces_by_color[color]["borders"][p_c] = []
                provinces_by_color[color]["borders"][p_c].append(border_id)

## Adjust borders

for border in global_borders:
    marked_for_removal_index = []
    
    last_removed = False
    for pixel_index in range(1, len(border)-1):
        center_pixel = border[pixel_index]
        
        vert_pixels = [
            (center_pixel[0], center_pixel[1] + 1),
            (center_pixel[0], center_pixel[1] - 1)
        ]
        
        hori_pixels = [
            (center_pixel[0] + 1, center_pixel[1]),
            (center_pixel[0] - 1, center_pixel[1]),
        ]
        
        adjacent_pixels = vert_pixels + hori_pixels
        
        is_adjacent = border[pixel_index - 1] in adjacent_pixels and border[pixel_index + 1] in adjacent_pixels
        is_vert = border[pixel_index - 1] in vert_pixels and border[pixel_index + 1] in vert_pixels
        is_hori = border[pixel_index - 1] in hori_pixels and border[pixel_index + 1] in hori_pixels
        is_ortho = is_vert + is_hori
        
        if is_ortho or (is_adjacent and not last_removed):
            marked_for_removal_index.insert(0, pixel_index)
            last_removed = True
        else:
            last_removed = False
            
    for pixel_index in marked_for_removal_index:
        border.pop(pixel_index)

for border_id in range(len(global_borders)):
    border = global_borders[border_id]
    new_border = [border[0], border[1]]
    
    for pixel_index in range(2, len(border)-2):
        last_pixel = border[pixel_index - 1]
        current_pixel = border[pixel_index]
        next_pixel = border[pixel_index + 1]
        
        
        if not ((last_pixel[0] == current_pixel[0] or last_pixel[1] == current_pixel[1]) and (next_pixel[0] == current_pixel[0] or next_pixel[1] == current_pixel[1])):
            new_border.append(current_pixel)
        else:
            for other_pixel in [last_pixel, next_pixel]:
                if other_pixel[0] == current_pixel[0] and other_pixel[1] > current_pixel[1]:
                    new_border.append((current_pixel[0], current_pixel[1] + 1))
                if other_pixel[0] == current_pixel[0] and other_pixel[1] < current_pixel[1]:
                    new_border.append((current_pixel[0], current_pixel[1] - 1))
                if other_pixel[0] > current_pixel[0] and other_pixel[1] == current_pixel[1]:
                    new_border.append((current_pixel[0] + 1, current_pixel[1]))
                if other_pixel[0] < current_pixel[0] and other_pixel[1] == current_pixel[1]:
                    new_border.append((current_pixel[0] - 1, current_pixel[1]))
    
    new_border.append(border[-2])
    new_border.append(border[-1])
    global_borders[border_id] = new_border

empty_provinces = []
for p_c in provinces_by_color:
    province = provinces_by_color[p_c]
    border_ids = [x for xs in list(province["borders"].values()) for x in xs]
    borders = [global_borders[x] for x in border_ids]

    if len(borders) == 0:
        empty_provinces.append(p_c)
        continue
    
    longest_border = 0
    for index, border in enumerate(borders):
        if False and len(border) > len(borders[longest_border]):
            longest_border = index
    province["main-border"].extend(borders.pop(longest_border))
    
    tries = 0
    while len(borders) >= 1 and tries < 1000:
        fixes = 0
        
        remaining_borders = borders.copy()
        for border_set in remaining_borders:
            if border_set[0] == province["main-border"][-1]:
                province["main-border"].extend(border_set)
                borders.remove(border_set)
                fixes += 1
            elif border_set[-1] == province["main-border"][0]:
                province["main-border"][:0] = border_set
                borders.remove(border_set)
                fixes += 1
            elif border_set[0] == province["main-border"][0]:
                province["main-border"][:0] = border_set[::-1]
                borders.remove(border_set)
                fixes += 1
            elif border_set[-1] == province["main-border"][-1]:
                province["main-border"].extend(border_set[::-1])
                borders.remove(border_set)
                fixes += 1
        
        if fixes <= 0:
            attempt_border = 0
            finished = False
            
            min_difference = 0#tries
            min_border = None
            
            while attempt_border < len(borders) and not finished:
                final_border = borders[attempt_border]
                shared_points = list(set(final_border).intersection(province["main-border"]))
                
                if len(shared_points) == 0:
                    first_difference = get_distance(final_border[0], province["main-border"][0])
                    second_difference = get_distance(final_border[-1], province["main-border"][0])
                    third_difference = get_distance(final_border[0], province["main-border"][-1])
                    fourth_difference = get_distance(final_border[-1], province["main-border"][-1])
                    
                    smallest_difference = min(first_difference, second_difference, third_difference, fourth_difference)
                    if smallest_difference < min_difference:
                        min_difference = smallest_difference
                        
                        if first_difference <= second_difference and first_difference <= third_difference and first_difference <= fourth_difference:
                            min_border = (0, 0, attempt_border)
                        if second_difference <= first_difference and second_difference <= third_difference and second_difference <= fourth_difference:
                            min_border = (-1, 0, attempt_border)
                        if third_difference <= second_difference and third_difference <= first_difference and third_difference <= fourth_difference:
                            min_border = (0, -1, attempt_border)
                        if fourth_difference <= second_difference and fourth_difference <= third_difference and fourth_difference <= first_difference:
                            min_border = (-1, -1, attempt_border)
                
                    attempt_border += 1
                else:
                    shared_point = final_border.index(shared_points[0])
                    before, after = final_border[0:shared_point], final_border[shared_point+1:]
                    finished = True
                    
                    if len(before) > 0 and len(after) > 0:                    
                        if len(before) > len(after):
                            borders[attempt_border] = before + [shared_points[0]]
                        else:
                            borders[attempt_border] = [shared_points[0]] + after
                    else:
                        shared_main_point = province["main-border"].index(shared_points[0])
                        main_before, main_after = province["main-border"][0:shared_main_point], province["main-border"][shared_main_point+1:]
                        
                        if len(main_before) > len(main_after):
                            province["main-border"] = main_before + [shared_points[0]]
                        else:
                            province["main-border"] = [shared_points[0]] + main_after
            
            if not finished and min_border != None:
                borders[min_border[2]][min_border[0]] = province["main-border"][min_border[1]]
            elif not finished:
                tries += 1

for province in empty_provinces:
    del provinces_by_color[province]

provinces_by_id = {}
id_by_color = {}
missing_ids = []
with open("color_id_lillie.csv", "r") as f:
    for line in f:
        R, G, B, id, is_ocean = line.strip().split(",")
        R, G, B, id, is_ocean = int(R), int(G), int(B), int(id), bool(is_ocean)
        
        next_color = (R,G,B,0)
        iterations = 0
        provinces = []
        while next_color in provinces_by_color:
            provinces.append(provinces_by_color[next_color])
            id_by_color[next_color] = id
            iterations += 1
            next_color = (R,G,B,iterations)
        
        if len(provinces) != 0:
            provinces_by_id[id] = provinces
        else:
            missing_ids.append(id)

with open("province_bodies.csv", "w") as f:
    for index, data in enumerate(provinces_by_id.items()):
        id, provinces = data
        f.write(f"{id},")
        for province_id, province in enumerate(provinces):
            f.write(f"{province['global_id']}")
            if province_id != len(provinces) - 1:
                f.write(",")
        if index != len(provinces_by_id.items()) - 1:
            f.write("\n")

with open("provinces_borders.csv", "w") as f:
    for index, data in enumerate(provinces_by_id.items()):
        id, provinces = data
        
        for province_id, province in enumerate(provinces):
            for other_index, other_province_color in enumerate(province["borders"].keys()):
                id_string = -1
                if other_province_color != None and other_province_color in id_by_color:
                    id_string = id_by_color[other_province_color]
                f.write(f"{id},{id_string},")
                for border_index, border in enumerate(province["borders"][other_province_color]):
                    f.write(f"{border}")
                    if border_index != len(province["borders"][other_province_color]) - 1:
                        f.write(",")
                if other_index != len(province["borders"]) - 1:
                    f.write("\n")
            if index != len(provinces_by_id) - 1 or province_id != len(provinces) - 1:
                f.write("\n")

with open("borders_points.csv", "w") as f:
    for index, border in enumerate(global_borders):
        f.write(f"{index},")
        
        for pixel_index, pixel in enumerate(border):
            f.write(f"{pixel[0]},{pixel[1]}")
            
            if pixel_index != len(border) - 1:
                f.write(",")
        
        if index != len(global_borders) - 1:
            f.write("\n")

with open("bodies_points.csv", "w") as f:
    for index, province in enumerate(provinces_by_color.values()):
        f.write(f"{province['global_id']},")
        
        border = province["main-border"]
        for pixel_index, pixel in enumerate(border):
            f.write(f"{pixel[0]},{pixel[1]}")
            
            if pixel_index != len(border) - 1:
                f.write(",")
        
        if index != len(provinces_by_color) - 1:
            f.write("\n")