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
; Personnage : Lisa, Homer, Bart, Marge, Maggie, Moe, Lenny, Carl, Ralph
; Lieux : Home, School, Central, Bar, Hospital
(clear)

(deffacts faits
	(is-at Home Lisa 2)
	(is-at Home Bart 2)
	(is-at Home Maggie 2)

	(is-at Home Homer 4)
	(is-at Home Lenny 4)

	; Moe (Patient Zero) contamine Lisa
	(is-at School Moe 5)
	(is-at School Lisa 5)

	;Lisa contamine Marge
	(is-at Central Lisa 14)
	(is-at Central Marge 14)
	
	
	; Marge contamine pas Homer
	(is-at Bar Marge 22)
	(is-at Bar Homer 22)

	;Marge contamine Homer et Lenny
	(is-at Hospital Homer 25)
	(is-at Hospital Lenny 25)
	(is-at Hospital Marge 25)

	; Homer contamine pas galcheyuk
	(is-at Home Homer 26)
	(is-at Home Maggie 26)

	; Homer contamine Bart
	(is-at Home Homer 33)
	(is-at Home Bart 33)
	(is-at Home Lenny 33)


	(is-at Central Maggie 34)
	(is-at Central Lenny 34)

	(not-infected-at Marge 15)
	(not-infected-at Lenny 20)
	(not-infected-at Maggie 40)
	(not-infected-at Bart 10)

	(was-contagious-at Lenny 33)
	(was-contagious-at Bart 41)
	(has-ebola Bart)
	(has-headache-at Bart 43)
	(got-ebola Homer)
	(has-headache-at Marge 27)
	(is-vomiting-at Marge 29)
	(has-diarrhea-at Marge 31)
	(is-vomiting-at Homer 37)
	(is-bleeding-at Homer 37)
	(got-ebola Lenny)
	(has-diarrhea-at Moe 31)
	(has-ebola Moe)
)



(batch ../../../Lab01/rules.clp)
(reset)
(run)