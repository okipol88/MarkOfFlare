class DoIt {

public Show() {
    alert("Hello")
}

public static Load() {

    (window as any)["xrp"] = new DoIt()
}

}

DoIt.Load()