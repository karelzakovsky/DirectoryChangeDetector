Řešení detekkuje změny v adresáři souborů, kde uživatel zadá cestu k adresáři a aplikace porovná aktuální stav souborů s uloženým stavem z předchozí analýzy. 
Pokud došlo k nějakým změnám (přidání, změna nebo smazání souborů), zobrazí se výstup s těmito změnami.

Controller HomeController zpracovává požadavky, porovnává aktuální a uložený stav adresáře a detekuje změny.
Data o souborech (cesta, datum poslední změny, verze) jsou serializována do JSON souboru a používána pro porovnání.
Pokud došlo k nějakým změnám, tyto změny se uloží a zpětně se vrátí na frontend pro zobrazení uživateli.

Omezení:
Závislost na souborovém systému.
Uložení dat do JSON souboru může být neefektivní pro velké adresáře s mnoha soubory.
Bezpečnostní rizika: Pokud je aplikace nasazena na veřejný server, může být riziko zneužití, pokud by někdo manipuloval s cestami nebo načítáním souborů.
(Omezená podpora pro velké adresáře: Při velkém počtu souborů může analýza a porovnání zabrat značný čas a způsobit problém s výkonem.
Žádná kontrola přístupových práv: Aplikace neprovádí žádnou validaci nebo kontrolu přístupových práv pro soubory.)
