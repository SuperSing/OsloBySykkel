# OsloBySykkel
 Konsumerer åpent API https://oslobysykkel.no/apne-data/sanntid
 # Informasjon om Applikasjon
Applikasjon er en «Console App» (.Net Framework). Applikasjonen konsumerer åpent API https://oslobysykkel.no/apne-data/sanntid og henter navnet på stativet, antall tilgjengelige låser og ledige sykler i øyeblikket.
# Kjøring
Applikasjon er bygd opp slik at den kan utvides og kjøres i flere modus via parametere.
For rask testing, skal kjøring uten parameter hente navnet på stativet, antall tilgjengelige låser og ledige sykler . Resultat blir lagret som en tekst fil med default lokasjon "C:\OsloBysykkel\StasjonInfoOgStatus". Det er mulig å definere en annen Drive (disk) via App.config fila.
Etter man har pakket ut og bygget koden, så kan man kjøre OsloBysykkel.exe via command prompt. 
