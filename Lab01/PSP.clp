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

; Faits de base
(deffacts faits
	(is-dead zenon)
	(meurtre from-t 1 to-t 4)
	(meurtre at-loc bluemountain)
	(at-loc denis bluemountain from-t 18 to-t 20)
	(at-loc daniel bluemountain at-time 2)
	(meurtre instr couteau)
	(have daniel couteau)
)
(batch rules.clp)
(reset)
(run)