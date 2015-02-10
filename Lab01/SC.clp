; ************************  States  *************************
;
; ---------- The ones you can use in the scenarios ----------
;
; is-at ?place ?person ?time
;
; not-infected-at ?person ?time
;
; got-ebola ?person
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
(reset)

(deffacts faits

	(not-infected-at ralph 5)
	(not-infected-at bart 10)

	(is-at school bart 19)
	(is-at school ralph 19)

	(is-at school lisa 20)
	(is-at school bart 20)
	(is-at school ralph 20)

	(is-at hospital marge 20)
	(not-infected-at marge 20)

	(is-at hospital bart 26)
	(is-at hospital marge 26)
	(is-at hospital homer 26)

	(not-infected-at homer 33)

	(has-headache-at ralph 30)
	(has-diarrhea-at ralph 32)
	(is-vomiting-at ralph 34)

	(has-diarrhea-at marge 38)
	(is-dead-at marge 42)
)

(batch "..\\..\\..\\Lab01\\rules.clp")
