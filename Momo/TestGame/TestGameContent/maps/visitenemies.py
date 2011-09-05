#!/bin/python

import xml.dom.minidom
import sys

if len(sys.argv) < 2:
    print "Usage: visitenemies.py filename"

filename = sys.argv[1]

doc = xml.dom.minidom.parse(filename)

objectgroups = doc.getElementsByTagName("objectgroup")

enemyNum = 0

for objectgroup in objectgroups:
    if objectgroup.attributes["name"].value == "Wave01":
        for object in objectgroup.childNodes:
            if object.nodeType == object.ELEMENT_NODE:
                object.attributes["type"] = "Enemy"
                #object.attributes["name"] = "Enemy%(#)02d" % {"#" : enemyNum}
                #enemyNum = enemyNum + 1

print doc.toprettyxml()