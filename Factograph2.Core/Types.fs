namespace Factograph2.Core

(* 
    ATOMS AND VARIABLES

    Atoms are just identifiers in Factograph, basically strings which you can only check for equality.
    Since atom and variable names are not required for any computation (only their identities), 
    we can later change them to ints for better performance.

    Examples of atoms: home, bob, cat.
    Examples of variables: X, Y, World.
*)
type AtomId = string
type VarId  = string

module IdEx =
    let AtomFromString str = str : AtomId
    let VarFromString str = str : VarId

(* 
    VARIANTS

    Variant is simply a value of any of several basic types - or just any .NET object.

    Examples of variants: 42, 3.14, "hello world".
*)
type Variant =
    | Integer of i : int
    | String  of s : string
    | Bool    of b : bool
    | Float   of f : double
    | Object  of o : obj

    static member New (i : int)    = Variant.Integer(i)
    static member New (s : string) = Variant.String(s)
    static member New (b : bool)   = Variant.Bool(b)
    static member New (f : double) = Variant.Float(f)

    static member New (o : obj) = 
        match o with
            | :? int    as i -> Variant.Integer i
            | :? string as s -> Variant.String s
            | :? bool   as b -> Variant.Bool b
            | :? float  as f -> Variant.Float f
            | _ -> Variant.Object o

    member self.Value : obj =
        match self with
            | Integer(i) -> i :> obj
            | String(s) -> s :> obj
            | Bool(b) -> b :> obj
            | Float(f) -> f :> obj
            | Object(o) -> o

    override self.ToString() = self.Value.ToString()

(*
    TERMS

    Terms are basic building blocks of Factograph, similar to statements or function calls in other languages.
    Values of all variables are terms.
    Terms can be simple or complex, creating a tree-like expression structure.

    Examples of terms: 42, bob, X, age(bob, 42), age(bob, X), square(origin(X, 10), size(20, Y)).
*)
type Term = 
    | Atom     of id : AtomId
    | Constant of value : Variant
    | Variable of name : VarId
    | Complex  of Term list

    static member New (i : int)    = Term.Constant(Variant.Integer(i))
    static member New (s : string) = Term.Constant(Variant.String(s))
    static member New (b : bool)   = Term.Constant(Variant.Bool(b))
    static member New (f : double) = Term.Constant(Variant.Float(f))

    static member New (c : Variant)   = Term.Constant(c)
    static member New (t : Term seq) = Term.Complex(Seq.toList t)

    member self.IsPredicate = 
        match self with
            | Complex (Atom _ :: _) -> true
            | _ -> false
    
    member self.Contains other =
        match self with
            | x when x = other -> true
            | Term.Complex(xs) -> 
                List.exists (fun (x: Term) -> x.Contains(other)) xs
            | _ -> false

    override self.ToString() =
        match self with
            | Atom(id)
            | Variable(id) -> id
            | Constant(c) -> c.ToString()
            | Complex(ts) -> "(" + System.String.Join(", ", ts) + ")"