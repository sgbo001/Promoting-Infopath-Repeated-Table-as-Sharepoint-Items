using Microsoft.Office.InfoPath;
using System;
using System.Xml;
using System.Xml.XPath;

namespace Requision_Form
{
    public partial class FormCode
    {
        // Member variables are not supported in browser-enabled forms.
        // Instead, write and read these values from the FormState
        // dictionary using code such as the following:
        //
        // private object _memberVariable
        // {
        //     get
        //     {
        //         return FormState["_memberVariable"];
        //     }
        //     set
        //     {
        //         FormState["_memberVariable"] = value;
        //     }
        // }

        // NOTE: The following procedure is required by Microsoft InfoPath.
        // It can be modified using Microsoft InfoPath.
        public void InternalStartup()
        {
            EventManager.XmlEvents["/my:myFields/my:AddContacts"].Changed += new XmlChangedEventHandler(AddContacts_Changed);
            EventManager.XmlEvents["/dfs:myFields/dfs:dataFields/d:SharePointListItem_RW", "MyContacts"].Changed += new XmlChangedEventHandler(MyContacts__SharePointListItem_RW_Changed);
            ((ButtonEvent)EventManager.ControlEvents["btnSendChanges_Clicked"]).Clicked += new ClickedEventHandler(btnSendChanges_Clicked_Clicked);
        }

        public void AddContacts_Changed(object sender, XmlEventArgs e)
        {
            try
            {
                // Get a reference to the form's XmlNamespaceManager object.
                XmlNamespaceManager ns = this.NamespaceManager;

                // Create an XPathNavigator object for the form's main data
                // source.
                XPathNavigator xnDoc = this.MainDataSource.CreateNavigator();

                // If the check box is set to true, add a new row so the user 
                // can add a new contact.
                if (e.Site.Value == "true")
                {
                    XPathNavigator xnTable =
                       xnDoc.SelectSingleNode("/my:myFields/my:gpContacts", ns);
                    xnTable.AppendChild("<my:gpContact><my:Title /><my:Description /><my:ItemNo /><my:Quantity /><my:UnitPrice /><my:Amount /><my:Vat /><my:TotalCost /><my:PONo /><my:TrackingID /></my:gpContact>");
                }
                else
                {
                    // If the user clears the check box, remove the added row.
                    ClearEnteredValues(xnDoc);
                }
            }
            catch (Exception ex)
            {

                throw;
            }// Write your code here to change the main data source.
        }
        private void ClearEnteredValues(XPathNavigator xnDoc)
        {
            try
            {
                // Get a reference to the form's XmlNamespaceManager object.
                XmlNamespaceManager ns = this.NamespaceManager;

                // Create an XPathNodeIterator object to get a count of the 
                // rows in the repeating table used to add new Contacts.
                XPathNodeIterator xi = xnDoc.Select(
                   "/my:myFields/my:gpContacts/my:gpContact", ns);
                int rowCount = xi.Count;

                if (rowCount > 0)
                {
                    // Get the first and last rows (nodes) in the 
                    // repeating table.
                    XPathNavigator firstNode = xnDoc.SelectSingleNode(
                       "/my:myFields/my:gpContacts/my:gpContact[1]", ns);
                    XPathNavigator lastNode = xnDoc.SelectSingleNode(
                       "/my:myFields/my:gpContacts/my:gpContact[" +
                       rowCount + "]", ns);

                    // Delete the existing nodes using the DeleteRange method.
                    firstNode.DeleteRange(lastNode);
                }

                // Clear the check box. 
                xnDoc.SelectSingleNode(
                   "/my:myFields/my:AddContacts", ns).SetValue("false");
            }

            catch (Exception ex)
            {

                throw;
            }
        }

        public void MyContacts__SharePointListItem_RW_Changed(object sender, XmlEventArgs e)
        {
            try
            {
                // Get a reference to the form's XmlNamespaceManager object.
                XmlNamespaceManager ns = this.NamespaceManager;

                // See if values have been changed.
                if (e.Operation == XmlOperation.ValueChange)
                {
                    // Set the Changed field to "true" only if the Delete
                    // check box is not selected.
                    if (e.Site.SelectSingleNode("@Delete", ns).Value != "true")
                    {
                        e.Site.SelectSingleNode("@Changed", ns).SetValue("true");
                    }
                }
            }
            catch (Exception ex)
            {

            }// Write your code here to change the main data source.
        }

        public void btnSendChanges_Clicked_Clicked(object sender, ClickedEventArgs e)
        {
            try
            {
                // Get a reference to the form's XmlNamespaceManager object.
                XmlNamespaceManager ns = this.NamespaceManager;

                // Create a DataSource object for the MyContacts 
                // list data connection.
                DataSource dsContacts = this.DataSources["MyContacts"];

                // Create an XPathNavigator object for the MyContacts 
                // data connection.
                XPathNavigator xnContacts = dsContacts.CreateNavigator();

                // Create an XPathNodeIterator object for the MyContacts 
                // data connection to enumerate the existing contacts.
                XPathNodeIterator xiContacts = xnContacts.Select(
                   "/dfs:myFields/dfs:dataFields/dfs:MyContacts", ns);

                // Create a DataSource object for the Add List Item Template
                // data connection.
                DataSource dsCAML = this.DataSources["Add List Item Template"];

                // Create an XPathNavigator object for the 
                // Add List Item Template data connection.
                // This is used to set the values in the CAML XML file.
                XPathNavigator xnCAML = dsCAML.CreateNavigator();

                // Create a WebServiceConnection object for submitting 
                // to the Lists Web service data connection.
                WebServiceConnection wsSubmit =
                   (WebServiceConnection)this.DataConnections[
                   "Web Service Submit"];

                // Create an XPathNavigator object for the form's 
                // main data source.
                XPathNavigator xnDoc = this.MainDataSource.CreateNavigator();

                // Create an XPathNodeIterator object for the new contacts.
                XPathNodeIterator xiNewContacts = xnDoc.Select(
                   "/my:myFields/my:gpContacts/my:gpContact", ns);

                // See if any new contacts have been added.
                if (xiNewContacts.Count > 0)
                {
                    while (xiNewContacts.MoveNext())
                    {
                        // Set the values in the Add List Item Template 
                        // XML file using the values in the new row.
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Title']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:Title", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Description']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:Description", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='ItemNo']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:ItemNo", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Quantity']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:Quantity", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='UnitPrice']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:UnitPrice", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Amount']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:Amount", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Vat']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:Vat", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='TotalCost']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:TotalCost", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='PONo']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:PONo", ns).Value);
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='TrackingID']", ns)
                           .SetValue(xiNewContacts.Current.SelectSingleNode(
                           "my:TrackingID", ns).Value);

                        // Set the values of the Changed and Delete columns to
                        // "FALSE".
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Changed']", ns)
                           .SetValue("FALSE");
                        xnCAML.SelectSingleNode(
                           "/Batch/Method/Field[@Name='Delete']", ns)
                           .SetValue("FALSE");

                        // Set the value of Cmd attribute to "New".
                        xnCAML.SelectSingleNode("/Batch/Method/@Cmd", ns)
                           .SetValue("New");

                        // Submit the new row.
                        wsSubmit.Execute();
                    }
                }
                else
                {
                    // Enumerate the existing contacts and see if 
                    // any items have been changed or set to be deleted.
                    while (xiContacts.MoveNext())
                    {
                        if (xiContacts.Current.SelectSingleNode(
                           "@Changed", ns).Value == "true")
                        {
                            // Set the values in the Add List Item Template XML file
                            // to the values in the updated row.
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='Title']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@Title").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='Description']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@Description").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='ItemNo']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@ItemNo").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='Quantity']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@Quantity").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='UnitPrice']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@UnitPrice").Value);
                            xnCAML.SelectSingleNode(
                              "/Batch/Method/Field[@Name='Amount']", ns)
                              .SetValue(xiContacts.Current.SelectSingleNode(
                              "@Amount").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='Vat']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@Vat").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='TotalCost']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@TotalCost").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='PONo']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@PONo").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='TrackingID']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@TrackingID").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='Changed']", ns)
                               .SetValue("false");
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='ID']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@ID").Value);

                            // Set the value of the Cmd attribute to "Update".
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/@Cmd", ns)
                               .SetValue("Update");

                            // Submit the updates for the current row.
                            wsSubmit.Execute();
                        }
                        // If the Delete field is set to "true" for the current
                        // row, in the Batch fragment set the ID for the row
                        // and set the Cmd attribute to "Delete".
                        if (xiContacts.Current.SelectSingleNode(
                           "@Delete", ns).Value == "true")
                        {
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/Field[@Name='ID']", ns)
                               .SetValue(xiContacts.Current.SelectSingleNode(
                               "@ID").Value);
                            xnCAML.SelectSingleNode(
                               "/Batch/Method/@Cmd", ns)
                               .SetValue("Delete");

                            // Submit the delete operation for the current row.
                            wsSubmit.Execute();
                        }
                    }
                }

                // Create a DataConnection object for the MyContacts 
                // data connection.
                DataConnection dcContacts = this.DataConnections["MyContacts"];

                // Execute the data connection to refresh the list of 
                // contacts to reflect any additions, updates, or deletions.
                dcContacts.Execute();

                // Clear the values in the repeating table for 
                // adding new contracts.
               
            }

            catch (Exception ex)
            {
               
                throw;
            }// Write your code here.
        }
    }
}
