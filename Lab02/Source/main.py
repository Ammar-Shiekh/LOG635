import unicodedata
import nltk
from nltk import *

#Ouvre le fichier de l'histoire
file = open("../texte.txt", "r")

#aeiouy
#Array d'opération à changer
remplacements=[["Marge ","LOL "],[",",". "],["’","'"],["'a"," a"],["'à"," à"],["'e"," e"],["'é"," é"],["'i"," i"],["'o"," o"],["'h"," h"],["'y"," y"],["'u"," u"],["\n", ""],["-", ""],["!", "."],["?", "."],["(", ""],[")", ""],[";", ". "]]
#voir http://la-conjugaison.nouvelobs.com/regles/grammaire/les-determinants-possessifs-79.php

#Print le contenu du fichier
texte = file.read()

#Cette opération permet de mettre toute la phrase en minuscule
texte = texte.lower()

#Ces opérations permet d'enlever les accents
texte = unicodedata.normalize("NFKD", texte)

#Parcour le tableau de remplacement et effectue les remplacements
for remplacement in remplacements:
   texte = texte.replace(remplacement[0],remplacement[1])

texte = texte.encode("ascii", "ignore")

#Permet de remettre le data en string
texte = texte.decode("utf-8")








print(texte)
print("clean")

tokens = "Lisa a mal a la tete a 3 heures".split()

with open ("../grammaire/Gramm.cfg", "r") as myfile:
    grammaireText=myfile.read()

grammar = grammar.FeatureGrammar.fromstring(grammaireText)
parser = nltk.ChartParser(grammar)

parser = parse.FeatureEarleyChartParser(grammar)
trees = parser.parse(tokens)
for tree in trees:
    print(tree)
    nltk.draw.tree.draw_trees(tree)
    print(tree.label()['SEM'])