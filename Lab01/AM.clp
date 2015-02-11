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
;Personnage : Homer,Marge,Maggie,Bart,Lisa,Karl,Lenny,Ralph,Moe
(deffacts fait


	
	; Aucune transmision 
	(is-at central homer 5)
	(is-at central lenny 5)
	(is-at central karl 5)
	
	
	; Transmistion marge -> homer
	(is-at home marge 8 )
	(is-at home homer 8 )
		
	;; maggie  l'attrape de marge
	
	(is-at hospital maggie 10)
	(is-at hospital marge 10)
	
	;; maggie le donne au enfants
	(is-at school lisa 18)
	(is-at school bart 18)
	(is-at school ralph 18)
	(is-at school maggie 18)
	
	; Tout le monde la deja
	(is-at home marge 42)
	(is-at home homer 42)
	(is-at home lisa 42)
	
	; transmission homer au bar 
	(is-at bar karl 50)
	(is-at bar lenny 50)
	(is-at bar homer 50)
	(is-at bar moe 50)
	(has-ebola marge)
	(not-infected-at homer 5)
	(has-headache-at lisa 20)
	(was-contagious-at maggie 18)

	
	
	

)

(batch ../../../Lab01/rules.clp)
(reset)
(run)