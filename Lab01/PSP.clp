;;;======================================================
;;;   Ebola Attack
;;;
;;;     This is an extended version of a
;;;     rather common AI planning problem.
;;;     The point is to find who is the
;;;		patient zero
;;;
;;;======================================================
;;;

;;;******************************************
;;;* TEMPLATES POUR LES FAITS DE DÉDUCTION *
;;;******************************************
: Personnage Price, Gallagher, Markov, Subban, Galchenyuk, Desharnais, Pacioretty

(clear)

(deffacts faits
	(is-at Entrainement Price 6)
	(is-at Entrainement Markov 6)
	(is-at Entrainement Galchenyuk 6)

	(is-at Entrainement Gallagher 7)
	(is-at Entrainement Pacioretty 7)

	; Desharnais (Patient Zero) contamine Price
	(is-at Gym Desharnais 9)
	(is-at Gym Price 9)

	;Price contamine subban
	(is-at MaisonCarey Price 17)
	(is-at MaisonCarey Subban 17)

	; Subban contamine pas gallagher
	(is-at Cinema Subban 22)
	(is-at Cinema Gallagher 22)

	;subban contamine gallagher et pacioretty
	(is-at Metro Gallagher 25)
	(is-at Metro Pacioretty 25)
	(is-at Metro Subban 25)

	; gallagher contamine pas galcheyuk
	(is-at Entrainement Gallagher 26)
	(is-at Entrainement Galchenyuk 26)

	; gallagher contamine markov
	(is-at Entrainement Gallagher 33)
	(is-at Entrainement Markov 33)
	(is-at Entrainement Pacioretty 33)


	(is-at Restaurant Galchenyuk 34)
	(is-at Restaurant Pacioretty 34)

	(not-infected-at Subban 15)
	(not-infected-at Pacioretty 20)
	(not-infected-at Galchenyuk 40)

	(was-contagious-at Pacioretty 41)
	(has-ebola Markov)
	;(got-ebola Price)
	(has-headache-at Markov 43)
	(got-ebola Gallagher 43)
)



(batch ../../LOG635/Lab01/rules.clp)
(reset)
(run)