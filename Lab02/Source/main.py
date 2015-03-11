import unicodedata
import nltk
from nltk import *

#Ouvre le fichier de l'histoire
fileReader = open("../texte.txt", "r")
fileWriter = open("../rules.txt", "w")

#Array d'opération à changer
inputRemplacement=[["et ",". "],[",",". "],["’","'"],["'a"," a"],["'à"," à"],["'e"," e"],["'é"," é"],["'i"," i"],["'o"," o"],["'h"," h"],["'y"," y"],["'u"," u"],["\n", " "],["-", ""],["!", "."],["?", "."],["(", ""],[")", ""],[";", ". "]]
#voir http://la-conjugaison.nouvelobs.com/regles/grammaire/les-determinants-possessifs-79.php

#Print le contenu du fichier
texte = fileReader.read()

#Cette opération permet de mettre toute la phrase en minuscule
texte = texte.lower()

#Ces opérations permet d'enlever les accents
texte = unicodedata.normalize("NFKD", texte)

#Parcour le tableau de remplacement et effectue les remplacements
for i in inputRemplacement:
   texte = texte.replace(i[0],i[1])

texte = texte.encode("ascii", "ignore")

#Permet de remettre le data en string
texte = texte.decode("utf-8")

with open ("../grammaire/grammaire.cfg", "r") as myfile:
    grammaireText=myfile.read()


#Instanciation des outils de NLTK
grammar = grammar.FeatureGrammar.fromstring(grammaireText)
parser = nltk.ChartParser(grammar)

phrases = re.split(r'\.(?!\d)', texte)
parser = parse.FeatureEarleyChartParser(grammar)

#Prénom précédent
name = ""
#Pour chaque phrase, nous instancions un arbre
for phrase in phrases:
    #tokens = "lisa a mal a la tete a 3 heures".split()
    tokens = phrase.split()
    trees = parser.parse(tokens)

    #On génère un arbre global
    for tree in trees:

        #nltk.draw.tree.draw_trees(tree)
        rule = str(tree.label()['SEM'])

        #Si le nom propre est 'ASD', on le remplace par le nom propre de la phrase précédente
        if( str(tree.label()['SUJ']) == "ASD"):
            rule = rule.replace(str(tree.label()['SUJ']), str(name))
        else:
            name = str(tree.label()['SUJ'])

        #On écrit la règle Jess
        remplacements = [['(', ' '], [')', ')\n('], [',', ' ']]
        for remplacement in remplacements:
            rule = rule.replace(remplacement[0], remplacement[1])

        rule = '(' + rule[:-1]

        #Écrit la règle dans le fichier rules.txt
        line = rule
        fileWriter.write(line)

print("Done")
fileWriter.close()