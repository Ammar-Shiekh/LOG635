import unicodedata

#Ouvre le fichier de l'histoire
file = open("../twat.txt", "r")

#Array d'opération à changer
remplacements=[[" et ",". "],[", ",". "],["qu'","que "],["n'", "ne "],["t'", "te "],["s'il", "si il "],["s'", "se "],["l'", "le "], ["m'", "me "],["j'", "je "],["d'", "de "],
               ["c'", "cela "],["\n", ""],["-", ""],["!", "."],["?", "."],["(", ""],[")", ""],[";", ". "]]

#Print le contenu du fichier
texte = file.read()

#Ces opérations permet d'enlever les accents
texte = unicodedata.normalize("NFKD", texte)
texte = texte.encode("ascii", "ignore")

#Cette opération permet de mettre toute la phrase en minuscule
texte = texte.lower()

#Permet de remettre le data en string
texte = texte.decode("utf-8")

#Parcour le tableau de remplacement et effectue les remplacements
for remplacement in remplacements:
   texte = texte.replace(remplacement[0],remplacement[1])

print(texte)
print("clean")

#grammar = grammar.FeatureGrammar.fromstring(grammaireText)
#parser = nltk.ChartParser(grammar)

#parser = parse.FeatureEarleyChartParser(grammar)
#trees = parser.parse(texte)
#for tree in trees:
#    print(tree)
#    nltk.draw.tree.draw_trees(tree)
#    print(tree.label()['SEM'])