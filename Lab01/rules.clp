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
;;;* TEMPLATES POUR LES R�GLES DE D�DUCTION *
;;;******************************************

; Suppression des faits
(clear)


; R�gle de d�part
(defrule startup
    =>
    (readline)
    (printout t "Laboratoire de l'�quipe X! R�gle de d�part" crlf)
    (assert (started))
)

; R�gles de d�duction
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