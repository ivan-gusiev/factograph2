namespace Factograph2.Core

(*
    VARIABLES CONTEXT

    Whenever we're asking questions in Factograph, we usually want to know not only True or False,
    but also values of variables in which the statement is true.

    This type encapsulates list of all variables which are "set" to a value.
*)
type Vars (value: Map<VarId, Term>) =
    member self.Value = value

    member self.WithVar id value =
        self.Value.Add (id value) |> Vars

    static member Empty = Vars Map.empty


type UnificationResult = 
    | Success of Vars
    | Failure

    static member Unify (lhs : Term, rhs : Term, context : Vars) =
        match (lhs, rhs) with
            | (Atom(s1), Atom(s2)) -> 
                if (s1 = s2) 
                    then UnificationResult.Success context 
                    else UnificationResult.Failure
            | _ -> UnificationResult.Failure

    static member Unify (lhs : Term, rhs : Term) = UnificationResult.Unify(lhs, rhs, Vars.Empty)