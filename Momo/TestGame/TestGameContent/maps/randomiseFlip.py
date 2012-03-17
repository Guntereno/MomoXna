#!/bin/python

import xml.dom.minidom
import sys
import random

if len(sys.argv) < 4:
    print "Usage: random_fill.py filename"

filenameArg = sys.argv[1]

doc = xml.dom.minidom.parse(filenameArg)

layerNodes = doc.getElementsByTagName("layer")

for layerNode in layerNodes:
	dataNodes = layerNode.getElementsByTagName("data")
	for dataNode in dataNodes:
		childNodes = dataNode.childNodes
		for childNode in childNodes:
			if childNode.nodeType == childNode.TEXT_NODE:
				csv = childNode.nodeValue
				tiles = csv.split(",")
				newTiles = []
				
				for tile in tiles:
					val = 0
					
					if tile != "0":
						val = int(tile);
						sval = val % 2**32
						sval = sval ^ (random.randint(0, 1) << 31)
						sval = sval ^ (random.randint(0, 1) << 30)
						val = sval
						
					newTiles.append(str(val))
					
				csv = ",".join(newTiles)
				childNode.nodeValue = csv

print doc.toprettyxml()