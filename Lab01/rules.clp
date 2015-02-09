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
; started-being-contagious-at ?person ?time
;
; has-diarrhea-at ?person ?time
;
; is-vomiting-at ?person ?time
;
; has-headache-at ?person ?time
;
; is-bleeding-at ?person ?time
;
; is-dead-at ?person ?time
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

(clear)
(printout t crlf)

(deffacts locations
	
	(meeting marge homer 2)
	(meeting bart homer 2)
	(meeting maggie homer 1)

)

(deffacts states

	(not-infected-at bart 3)
	(not-infected-at maggie 3)

	(has-headache-at homer 12)
	(is-dead-at homer 14)
	(got-ebola homer)
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

	(printout t "(transmissionDroite) ")
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

	(printout t "(transmissionGauche) ")
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

	(not
		(and
			(not-infected-at ?transmitor ?nonInfectionTime2)
			(test (> (+ ?nonInfectionTime2 8) ?meetingTime))
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

			(test (>= ?otherMeetingTime ?meetingTime))			
			(not-infected-at ?anotherPerson ?nonInfectionTime)
			(test (>= ?nonInfectionTime ?otherMeetingTime))
		)
	)

	(not 															; and ?infected didn't meet with someone else
		(and
			(or
				(meeting ?infected ?someoneElse ?otherMeetingTime)
				(meeting ?someoneElse ?infected ?otherMeetingTime)
			)
			(not (test (= ?transmitor ?someoneElse)))

			(or 													; or someone else was declared not-infected after the meeting
				(and
					(not-infected-at ?someoneElse ?t5)
					(test (< ?t5 ?otherMeetingTime))
				)
				(not 
					(not-infected-at ?someoneElse ?t6)
				)
			)
		)
	)


	=>
	(assert (transmission ?transmitor ?infected ?meetingTime))

	(printout t "(transmissionDeduite) ")
	; go to transmissionSimple
)

;
; Deduces that ?transmitor gave ebola to ?infected if ?infected
; got ebola and he only had 1 meeting... to specify...
;
(defrule transmissionViaGotEbola

	(got-ebola ?infected)											; If got-ebola ?infected
	(or 															; and ?infected had a meeting with transmitor
		(meeting ?infected ?transmitor ?sometime)
		(meeting ?transmitor ?infected ?sometime)
	)
	
	(or 															; and ?infected was not not-infected after the meeting
		(and
			(not-infected-at ?infected ?t1)
			(test (< ?t1 ?sometime))
		)
		(not 
			(not-infected-at ?infected ?t2)
		)
	)

	(or 
		(and
			(not-infected-at ?transmitor ?t1)
			(test (< ?t1 ?sometime))
		)
		(not 
			(not-infected-at ?transmitor ?t2)
		)
	)

	(not 															; and ?infected didn't meet with someone else
		(and
			(or
				(meeting ?infected ?someoneElse ?othertime)
				(meeting ?someoneElse ?infected ?othertime)
			)
			(test (= ?sometime ?othertime))
			(test (= ?transmitor ?someoneElse))
			
			(or 													; or the meeting with someone else was after ?infected was declared not-infected
				(and
					(not-infected-at ?infected ?t3)
					(test (< ?t3 ?othertime))
				)
				(not 
					(not-infected-at ?infected ?t4)
				)
			)

			(or 													; or someone else was declared not-infected after the meeting
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
)

;
; Deducing with symptom if ?p have ebola
;
(defrule contagionFromMinorSymptoms

	(has-headache-at ?p ?t1)
	(has-diarrhea-at ?p ?t2)
	(is-vomiting-at ?p ?t3)

	(test (= ?t2 (+ ?t1 2)))
	(test (= ?t3 (+ ?t2 2)))

	=>
	(assert (started-being-contagious-at ?p (- ?t1 2)))
	(assert (was-contagious-at ?p (- ?t1 2)))
	(assert (has-ebola ?p))
)

(defrule contagionFromHeadacheAndMajorSymptom

	(or
		(is-dead-at ?person ?t1)
		(is-bleeding-at ?person ?t2)
	)
	(has-headache-at ?person ?t3)
	
	=>

	(assert (started-being-contagious-at ?person (- ?t3 2)))
	(assert (was-contagious-at ?person (- ?t3 2)))
	(assert (has-ebola ?person))
)

(defrule contagionFromDiarrheaAndMajorSymptom

	(or
		(is-dead-at ?person ?t1)
		(is-bleeding-at ?person ?t2)
	)
	(has-diarrhea-at ?person ?t3)
	
	=>
	(assert (started-being-contagious-at ?person (- ?t3 4)))
	(assert (was-contagious-at ?person (- ?t3 4)))
	(assert (has-ebola ?person))
)

(defrule contagionFromVomitingAndMajorSymptom

	(or
		(is-dead-at ?person ?t1)
		(is-bleeding-at ?person ?t2)
	)
	(is-vomiting-at ?person ?t3)
	
	=>
	(assert (started-being-contagious-at ?person (- ?t3 6)))
	(assert (was-contagious-at ?person (- ?t3 6)))
	(assert (has-ebola ?person))
)

;
; If we know when someone became contagious, than remove all future (was-contagious-at ...)
;
(defrule retractFutureWasContagious

	(started-being-contagious-at ?p ?contagionTime)
	(was-contagious-at ?p ?otherTime)
	(test (> ?otherTime ?contagionTime))

	?toRetract <- (was-contagious-at ?p ?otherTime)

	=>

	(retract (?toRetract))
)

(defrule transmissionViaStartedBeingContagious

	(started-being-contagious-at ?infected ?contagiousTime)
	(or
		(meeting ?infected ?transmitor ?transmissionTime)
		(meeting ?transmitor ?infected ?transmissionTime)
	)
	(test (= ?contagiousTime (+ ?transmissionTime 8)))

	(not 
		(and
			(or
				(meeting ?infected ?someoneElse ?transmissionTime)
				(meeting ?someoneElse ?infected ?transmissionTime)
			)
			(not (test (= ?someoneElse ?transmitor)))

			(or
				(and
					(not-infected-at ?someoneElse ?t5)
					(test (< ?t5 ?transmissionTime))
				)
				(not 
					(not-infected-at ?someoneElse ?t6)
				)
			)
		)
	)

	(not (transmission ?transmitor ?infected ?transmissionTime))

	=>
	(assert (transmission ?transmitor ?infected ?transmissionTime))

	(printout t "(transmissionViaStartedBeingContagious) ")
)

;
; TODO
;
(defrule gotEbolaFromNotInfected
	
	(not-infected-at ?person ?t1)
	(was-contagious-at ?person ?t2)

	=>
	(assert (got-ebola ?person))
)


(reset)
(run)
(printout t crlf)

;
; Asserts who is patient zero
;

(defrule patientZero
	(declare (salience 1))

	(has-ebola ?p1)
	(not (got-ebola ?p1))

	=>
	(assert (is-patient-zero ?p1))

	(printout t "LE PATIENT ZERO EST " ?p1 "! ARRETEZ LE !!!" crlf)
)

(run)

(printout t crlf)

(facts)