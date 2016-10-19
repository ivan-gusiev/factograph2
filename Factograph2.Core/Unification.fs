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
        self.Value.Add(id, value) |> Vars

    member self.Has id =
        self.Value.ContainsKey id

    member self.IsFree id = 
        not (self.Has id)

    static member Empty = Vars Map.empty


type UnificationResult = 
    | Success of Vars
    | Failure

    member self.Variables : seq<VarId * Term> =
        match self with 
            | Success(vars) -> Map.toSeq(vars.Value)
            | Failure -> Seq.empty

    static member Unify (lhs : Term, rhs : Term, context : Vars) =
        match (lhs, rhs) with
            | (Atom(s1), Atom(s2)) when s1 = s2 ->
                UnificationResult.Success context 

            | (Constant(c1), Constant(c2)) when c1 = c2 -> 
                UnificationResult.Success context 

            | (Variable(v1), Variable(v2)) when v1 = v2 ->
                UnificationResult.Success context
            
            // implement Occurs check, which disallows unifying expressions like X = f(X)
            | (Variable(vid), Complex(ts)) & (v, c) 
            | (Complex(ts), Variable(vid)) & (v, c) when Config.OccursCheck && c.Contains(v) ->
                    UnificationResult.Failure

            | (Variable(v), newValue) 
            | (newValue, Variable(v)) when context.IsFree(v) ->
                context.WithVar v newValue |> UnificationResult.Success

            | (Variable(v), _) ->
                UnificationResult.Failure
            
            | _ -> UnificationResult.Failure

    static member Unify (lhs : Term, rhs : Term) = UnificationResult.Unify(lhs, rhs, Vars.Empty)