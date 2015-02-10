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
; has-diarrhea-at ?person ?time
;
; is-vomiting-at ?person ?time
;
; has-headache-at ?person ?time
;
; is-bleeding-at ?person ?time
;

; *** Events ***
; 
; transmission ?transmitor ?infected ?time
;
; meeting ?person1 ?person2 ?time
;

; *** People *** 
;
; homer  lisa    lenny
;
; marge  maggie  carl
;
; bart   moe     ralph
;


;(printout t crlf)

;(deffacts faits
;	
;	(is-at home marge 5)
;	(is-at home homer 5)
;
;	(is-at central lenny 9)
;	(is-at central homer 9)
;	(is-at central carl 9)
;
;	(meeting lisa bart 4)
;	(meeting marge lisa 10)
;	(meeting lenny carl 8)
;	(meeting homer carl 10)
;	(meeting marge ralph 14)
;	(meeting maggie bart 12)
;	(meeting bart marge 15)
;	(meeting carl moe 14)
;	(meeting homer ralph 20)
;	(meeting lisa lenny 18)
;	(meeting ralph moe 24)
;
;	(got-ebola moe)
;	(not-infected-at lisa 5)
;	(not-infected-at moe 15)
;	(got-ebola marge)
;
;)

;
; Asserts has-ebola if got-ebola
;
(defrule haveEbolaIfGotIt
	(declare (salience 5))
	(got-ebola ?infected)
	(not (has-ebola ?infected))
	=>
	(assert (has-ebola ?infected))
	
	(printout t ?infected " has Ebola. (HaveEbolaIfGotIt)" crlf)
)

;
; Deduce got-ebola if the infected met someone 8 hours before
;
(defrule deduceGotEbola
	(declare (salience 40))
	(was-contagious-at ?infected ?t1)
	(meeting ?infected ?p2 ?t2)
	(not(got-ebola ?p2))
	(not(has-ebola ?p2))
	(not(got-ebola ?infected))
	(test (<= ?t2 (- ?t1 8)))
	=>
	(assert (got-ebola ?infected))
	
	(printout t ?infected " got Ebola. (deduceGotEbola)" crlf)
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

	(printout t ?transmitor " a donnee l'Ebola a " ?infected " a " ?t "h. (transmissionSimple)" crlf)
)

;
; Transmission during a meeting (?p1 infects ?p2)
;
(defrule transmissionDroite
	(declare (salience 25))
	(meeting ?p1 ?p2 ?t)
	(was-contagious-at ?p1 ?t2)
	(test (>= ?t ?t2))
	(not (has-ebola ?p2))
	=>
	(assert (transmission ?p1 ?p2 ?t))

	(printout t "(transmissionDroite) ")
	; go to transmissionSimple
)

;
; Transmission during a meeting (?p2 infects ?p1)
;
(defrule transmissionGauche
	(declare (salience 4))
	(meeting ?p1 ?p2 ?t)
	(was-contagious-at ?p2 ?t2)
	(test (>= ?t ?t2))
	(not (has-ebola ?p1))
	=>
	(assert (transmission ?p2 ?p1 ?t))

	(printout t "(transmissionGauche) ")
	; go to transmissionSimple
)

;
; Deduce that a transmission occured if ?infected has ebola and
; he met with ?transmitor
;
(defrule transmissionDeduite
	(declare (salience 15))
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
	(test (<= ?contagiousTime (+ ?meetingTime 8)))

	(not
		(and
			(not-infected-at ?transmitor ?nonInfectionTime2)
			(test (>= ?nonInfectionTime2 ?meetingTime))
		)
	)

	; If there was a meeting between ?transmitor and another person after
	; the current meeting and the other person is not infected after that,
	; than the ?transmitor could not be contagious now.
	(not 
		(and
			(or
				(meeting ?transmitor ?anotherPerson ?otherMeetingTime)
				(meeting ?anotherPerson ?transmitor ?otherMeetingTime)
			)
			(not-infected-at ?anotherPerson ?nonInfectionTime)
			(test (>= ?nonInfectionTime ?otherMeetingTime))
			(test (>= ?otherMeetingTime ?meetingTime))
		)
	)

	=>
	(assert (transmission ?transmitor ?infected ?meetingTime))

	(printout t "(transmissionDeduite) ")
	; go to transmissionSimple
)

;
; Deduces that ?transmitor gave ebola to ?infected if ?infected
; got ebola and he only had 1 meeting
;
(defrule transmissionViaGotEbola
	(declare (salience 10))
	(got-ebola ?infected)
	(or
		(meeting ?infected ?transmitor ?sometime)
		(meeting ?transmitor ?infected ?sometime)
	)
	
	(or 
		(and
			(not-infected-at ?infected ?t1)
			(test (< ?t1 ?sometime))
		)
		(not 
			(not-infected-at ?infected ?t2)
		)
	)

	(not
		(and
			(or
				(meeting ?infected ?someoneElse ?othertime)
				(meeting ?someoneElse ?infected ?othertime)
			)
			(not (test (= ?sometime ?othertime)))
			(not (test (= ?transmitor ?someoneElse)))
			
			(or 
				(and
					(not-infected-at ?infected ?t3)
					(test (< ?t3 ?othertime))
				)
				(not 
					(not-infected-at ?infected ?t4)
				)
			)

			(or 
				(and
					(not-infected-at ?someoneElse ?t5)
					(test (< ?t5 ?othertime))
				)
				(not 
					(not-infected-at ?someoneElse ?t6)
				)
			)
		)
	)

	=>
	(assert (transmission ?transmitor ?infected ?sometime))

	(printout t "(transmissionViaGotEbola) ")
	; go to transmissionSimple
)

;
; Removes was-contagious facts that are useless (after a previous)
;
(defrule removeFutureContagion
	
	(was-contagious-at ?person ?t1)
	(was-contagious-at ?person ?t2)
	(test (< ?t1 ?t2))
	?toRetract <- (was-contagious-at ?person ?t2)
	=>
	(retract ?toRetract)
)

;
; Removes not-infected-at facts that are useless (before one that is later)
;
(defrule removePreviousNonInfections
	
	(not-infected-at ?person ?t1)
	(not-infected-at ?person ?t2)
	(test (> ?t1 ?t2))
	?toRetract <- (not-infected-at ?person ?t2)
	=>
	(retract ?toRetract)
)

(defrule meetingViaLocation
	(declare (salience 35))
	(is-at ?lieu ?person1 ?t1)
	(is-at ?lieu ?person2 ?t1)
	(not (test (= ?person1 ?person2)))
	(not 
		(or
			(meeting ?person1 ?person2 ?t1)
			(meeting ?person2 ?person1 ?t1)
		)
	)
	=>
	(assert (meeting ?person1 ?person2 ?t1))

	(printout t  "Meeting entre " ?person1 " et " ?person2 " a " ?t1 "h. (meetingViaLocation)" crlf)
)

;
; Deducing with symptom if ?p have ebola
;
(defrule ebolaFromSymptoms
	(declare (salience 40))
	(has-headache-at ?p ?t1)
	(has-diarrhea-at ?p ?t2)
	(is-vomiting-at ?p ?t3)

	(test (= ?t1 (+ ?t2 2)))
	(test (= ?t2 (+ ?t3 2)))

	=>
	(assert (was-contagious-at ?p (- ?t1 2)))
	(assert (has-ebola ?p))
)

;
; Deducing contagious time with symptom has-headache-at if ?p have ebola
;
(defrule ebolaFromSymptomsHeadache
	(declare (salience 39))
	(has-ebola ?p)
	(has-headache-at ?p ?t1)
	=>
	(assert (was-contagious-at ?p (- ?t1 2)))
)

;
; Deducing contagious time with symptom has-diarrhea-at if ?p have ebola
;
(defrule ebolaFromSymptomsDiarrhea
	(declare (salience 39))
	(has-ebola ?p)
	(has-diarrhea-at ?p ?t1)
	=>
	(assert (was-contagious-at ?p (- ?t1 4)))
)

;
; Deducing contagious time with symptom has-vomiting-at if ?p have ebola
;
(defrule ebolaFromSymptomsVomiting
	(declare (salience 39))
	(has-ebola ?p)
	(has-vomiting-at ?p ?t1)
	=>
	(assert (was-vomiting-at ?p (- ?t1 6)))
)
;
; Asserts who is patient zero
;
(defrule patientZero

	(has-ebola ?p1)
	(was-contagious-at ?p1 ?t1)
	(meeting ?p1 ?p2 ?t2)
	
	(was-contagious-at ?p3 ?t3 )
	(not (is-patient-zero ?p1))
	(not(got-ebola ?p1))
	;is not patient-zero if meeting some one before being contagious
	(test (<= ?t1 ?t2 ))
	(not (test (< ?t3 ?t1)))
	=>
	(assert (is-patient-zero ?p1))

	(printout t ?p1 " et " ?p2 crlf)
	(printout t ?p1 " est le patient-zero! (patientZero)" crlf)
)

(reset)
(run)
(printout t crlf)
(facts)