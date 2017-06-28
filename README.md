# printeragent
eHealth printing utility desktop application

Printer agent provides a web service on a specific port what must be set up in Printer Configuration System (PCS) service.
Web service accepts document type, signature and document as input. Ehealth user interface components, running in local browser, can call print agent's web service to submit print jobs.
In successful cases print agent must respond with a dummy image (as small as possible) and forward received document to an appropriate printer configured in PCS.
