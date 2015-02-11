# -*- coding: utf-8 -*-

import nltk
from nltk import *

with open ("exemple2.cfg", "r") as myfile:
    grammaireText=myfile.read()

grammar = grammar.FeatureGrammar.fromstring(grammaireText)
parser = nltk.ChartParser(grammar)
tokens = "Jean tua Marie avec une corde".split()


parser = parse.FeatureEarleyChartParser(grammar)
trees = parser.parse(tokens)
for tree in trees:
    print(tree)
    nltk.draw.tree.draw_trees(tree)
    print(tree.label()['SEM'])
