# -*- coding: utf-8 -*-

import nltk
from nltk import *

with open ("exemple1.cfg", "r") as myfile:
    grammaireText=myfile.read()

grammar = grammar.FeatureGrammar.fromstring(grammaireText)
parser = nltk.ChartParser(grammar)
tokens = "Jean tua Marie".split()


parser = parse.FeatureEarleyChartParser(grammar)
trees = parser.parse(tokens)
texte = 'tuer(jean,marie)is-at(homer,marie, 8)'
for tree in trees:
    #print(tree)
    #nltk.draw.tree.draw_trees(tree)
    #print(tree.label()['SEM'])
    texte = tree.label()['SEM']
    #print(tree.index)

texte = str(texte)
remplacements = [['(', ' '], [')', ')\n('], [',', ' ']]
for remplacement in remplacements:
    texte = texte.replace(remplacement[0], remplacement[1])

texte = '(' + texte[:-1]
input(texte)



	
	

	
