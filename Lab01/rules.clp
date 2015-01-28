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


; *** States ***
; 
; has-ebola ?person
;
; not-infected-at ?person ?time
;
; got-ebola ?person
;
; was-contagious-at ?person ?time
;
; has-symptom ?person ?symptom
;
;
;

; *** Events ***
; 
; transmission ?transmitor ?infected ?time
;
; meeting ?person1 ?person2 ?time
;
;
;
;
;

(clear)
(printout t crlf)

(deffacts faits
	
	(meeting jim bob 2)
	(meeting jane bob 3)
	(meeting jane ginette 5)
	(got-ebola bob)
	(transmission bob simon 12)
	(meeting simon marc 13)
	(has-symptom lololol headache)
	(has-symptom lololol vomiting)
	(has-symptom lololol diarrhea)
)

;
; Asserts caused by transmission
;
(defrule transmissionSimple
	(declare (salience 30))

	(transmission ?transmitor ?infected ?t)
	=>
	(assert (has-ebola ?transmitor))
	(assert (was-contagious-at ?transmitor ?t))

	(assert (has-ebola ?infected))
	(assert (got-ebola ?infected))
	(assert (was-contagious-at ?infected (+ ?t 8)))

	(printout t ?transmitor " a donne l'Ebola a " ?infected " a " ?t "h. (transmissionSimple)" crlf)
)

;
; Transmission during a meeting (?p1 infects ?p2)
;
(defrule transmissionDroite
	(meeting ?p1 ?p2 ?t)
	(was-contagious-at ?p1 ?t2)
	(test (>= ?t ?t2))
	(not (has-ebola ?p2))
	=>
	(assert (transmission ?p1 ?p2 ?t))

	(printout t "(transmissionDroite) " crlf)
	; go to transmissionSimple
)

;
; Transmission during a meeting (?p2 infects ?p1)
;
(defrule transmissionGauche
	(meeting ?p1 ?p2 ?t)
	(was-contagious-at ?p2 ?t2)
	(test (>= ?t ?t2))
	(not (has-ebola ?p1))
	=>
	(assert (transmission ?p2 ?p1 ?t))

	(printout t "(transmissionGauche) " crlf)
	; go to transmissionSimple
)

;
; Deduce that a transmission occured if ?infected has ebola and
; he met with ?transmitor
;
(defrule transmissionDeduite
	(or 
		(meeting ?infected ?transmitor ?meetingTime)
		(meeting ?transmitor ?infected ?meetingTime)
	)
	(has-ebola ?infected)
	(got-ebola ?infected)
	(not (transmission ?transmitor ?infected ?anOtherTime))
	(not (transmission ?infected ?transmitor ?meetingTime))
	(not (transmission ?someoneElse ?infected ?anOtherTime2))

	(was-contagious-at ?infected ?contagiousTime)
	(test (>= ?contagiousTime (+ ?meetingTime 8)))
	=>
	(assert (transmission ?transmitor ?infected ?meetingTime))

	(printout t "(transmissionDeduite) " crlf)
	; go to transmissionSimple
)

;
; Removes was-contagious facts that are useless (after a previous)
;
(defrule removeFutureContagion
	
	(was-contagious-at ?person ?t1)
	(was-contagious-at ?person ?t2)
	(test (< ?t1 ?t2))
	?toRetract <- (was-contagious-at ?person ?t2 crlf)
	=>
	(retract ?toRetract)
)

;
; Asserts who is patient zero
;
(defrule patientZero
	(declare (salience 1))

	(has-ebola ?p1)
	(not (got-ebola ?p1))
	=>
	(assert (is-patient-zero ?p1))

	(printout t ?p1 " est le patient-zero! (patientZero)" crlf)
)

;
; Define symptom for headache
;
(defrule symptomHeadache
	(declare (salience 27))
	(was-contagious-at ?p ?t)
	(not (has-symptom ?p headache))
	(test (>= ?t 2))
	=>
	(assert (has-symptom ?p headache))

	(printout t ?p " a headache a "(+ ?t 2) crlf)
)

;
; Define symptom for diarrhea
;
(defrule symptomdiarrhea
	(declare (salience 26))
	(was-contagious-at ?p ?t)
	(not (has-symptom ?p diarrhea))
	(test (>= ?t 4))
	=>
	(assert (has-symptom ?p diarrhea))

	(printout t ?p " a diarrhea a "(+ ?t 4) crlf)
)

;
; Define symptom for vomiting
;
(defrule symptomvomiting
	(declare (salience 25))
	(was-contagious-at ?p ?t)
	(not (has-symptom ?p vomiting))
	(test (>= ?t 6))
	=>
	(assert (has-symptom ?p vomiting))

	(printout t ?p " a vomiting a "(+ ?t 6) crlf)
)

;
; Deducing with symptom if ?p have ebola
;
(defrule ebolaFromSymptom
	(has-symptom ?p headache)
	(has-symptom ?p diarrhea)
	(has-symptom ?p vomiting)
	(not (has-ebola ?p))
	=>
	(assert (has-ebola ?p))

	(printout t ?p " a l'ebola " crlf)
)

(reset)
(run)
