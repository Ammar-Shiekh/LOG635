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
;;;* TEMPLATES POUR LES RÈGLES DE DÉDUCTION *
;;;******************************************

; Suppression des faits
(clear)


; Règle de départ
(defrule startup
    =>
    (readline)
    (printout t "Laboratoire de l'équipe X! Règle de départ" crlf)
    (assert (started))
)

; Règles de déduction
(defrule transmission
	(declare (salience 60))
	(meurtre at-loc ?loc)
	(meurtre from-t ?t1 to-t ?t2)
	(meurtre instr ?inst)
	(at-loc ?s ?loc at-time ?t3)
	(test (>= ?t3 ?t1))
	(test (<= ?t3 ?t2))
	(have ?s ?inst)
	(started)
	=>
	(printout t "Voici le suspect: " ?s crlf)
	(assert (is-suspect ?s))
)
(reset)
(run)