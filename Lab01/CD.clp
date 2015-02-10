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

(is-at home lenny 5)
(is-at home carl  5)

(is-at hospital marge 5)
(is-at hospital maggie 5)

(is-at school homer 8)
(is-at school marge 8)
(is-at school bart 8)

(is-at bar ralph 8)
(is-at bar moe 8)

(is-at hospital lisa 8)
(is-at hospital carl 8)

(is-at central homer 12)
(is-at central lenny 12)

(is-at hospital carl 12)
(is-at hospital bart 12)

(is-at home marge 14)
(is-at home lisa 14)
(is-at home lenny 14)

(is-at bar homer 14)
(is-at bar ralph 14)

(is-at central bart 20)
(is-at central homer 20)
(is-at central lenny 20)
(is-at hospital marge 20)
(is-at hospital lisa 20)

(is-at home bart 24)
(is-at home maggie 24)

(is-at central homer 24)
(is-at central lenny 24)

(is-at bar ralph 28)
(is-at bar moe 28)
(is-at bar homer 28)

;(is-at school lenny 28)
;(is-at school carl 28)

(has-headache lisa 10)
(not-infected-at homer 10)
(not-infected-at maggie 20)

(was-contagious-at homer 28)
(got-ebola lenny)
;(got-ebola bart)
(not-infected-at bart 3)

(batch ../../LOG635/Lab01/rules.clp)
(reset)
(run)
