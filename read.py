#!/usr/bin/python3
# -*- coding:utf8 -*-
import codecs


with open('Log.txt', encoding='utf-8') as data_file:
    lines = data_file.readlines()

with open('Check.txt','wt') as f:
    for l in lines:
        words = l.split(" ")
        if words[0] == "Reduced" and words[1] == "MIP":
            for w in range(0, len(words)):
                print("{0},".format(words[w]), file=f)
    pass
