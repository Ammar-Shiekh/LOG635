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

;;;*************
;;;* TEMPLATES *
;;;*************


; *** Events ***
; 
; transmission ?transmitor ?infected ?time
;
; meeting ?person1 ?person2 ?time

; started-vomitting ?person ?time
;
; started-headaches ?person ?time
;
; started-shitting  ?person ?time
;
; got-ebola ?person
;

(clear)

(deffacts faits

	(meeting jane bob 3)
	(meeting bob simon 13)
	(transmission bob simon 13)

)

(defrule transmissionSimple
	(transmission ?transmitor ?infected ?t)
	=>
	(assert (has-ebola ?infected))
	(assert (got-ebola ?infected))
	(printout t ?transmitor " a donne l'Ebola a " ?infected " a " ?t "h" crlf)
)

(defrule transmissionDroite
	(meeting ?p1 ?p2 ?t)
	(is-contagious ?p1)
	(not (has-ebola ?p2))
	=>
	(assert (transmission ?p1 ?p2 ?t))
)

(defrule transmissionGauche
	(meeting ?p1 ?p2 ?t)
	(is-contagious ?p2)
	(not (has-ebola ?p1))
	=>
	(assert (transmission ?p2 ?p1 ?t))
)

(defrule transmissionInverse
	(transmission ?transmitor ?infected ?t1)
	(has-ebola ?infected)
	=>
	(assert (is-contagious ?transmitor))

	(printout t "On peut deduire que " ?transmitor " a l'ebola"  crlf)
)

(defrule contagion
	(transmission ?p1 ?p2 ?t1)
	(meeting ?p2 ?p3 ?t2)
	(test (>= ?t2 (+ ?t1 8))) ; CONTAGION TIME 8H !!!
	=>
	(assert (is-contagious ?p2))

	(printout t ?p2 " est devenu contagieux a " ?t2 "h" crlf)
)

(defrule ebolaViaContagion
	(declare (salience 5) ) ;RULE #5

	(is-contagious ?p1)
	(not (has-ebola ?p1))
	=>
	(assert (has-ebola ?p1))
)

(defrule patientZero
	(declare (salience 10) ) ;RULE #1 (derniere regle)

	(has-ebola ?p1)
	(not (got-ebola ?p1))
	=>
	(assert (is-patient-zero ?p1))
	(printout t ?p1 " est le patient-zero!" crlf)

)

(reset)
(run)