<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:key name="SourceItemKey" match="Event" use="@SourceItem"/>
  
    <xsl:variable name="Hints">
    <ManualActions>
      <xsl:for-each select="/UpgradeLog/ManualActions/ManualAction">
        <ManualAction>
          <xsl:attribute name="Message">
            <xsl:value-of select="@Message"/>
          </xsl:attribute>
        </ManualAction>
      </xsl:for-each>
    </ManualActions>
  </xsl:variable>

  <xsl:template match="ManualActions">
    <h2>
      Required Manual Actions
    </h2>
    <table cellpadding="2" cellspacing="0" width="98%" border="1" bordercolor="white" class="infotable">
      <tr>
        <td nowrap="1" class="header">
          Required manual actions:
        </td>
      </tr>
      <xsl:for-each select="ManualAction">
        <tr>
          <td nowrap="1" class="content">
            <xsl:value-of select="@Message"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
  
   <xsl:variable name="ErrorEvents">
    <ErrorEvents>
      <xsl:for-each select="/UpgradeLog/Errors/Error">
        <Error>
          <xsl:for-each select="Message">
            <Line>
              <xsl:attribute name="Message">
                <xsl:value-of select="."/>
              </xsl:attribute>
            </Line>
          </xsl:for-each>
        </Error>
      </xsl:for-each>
    </ErrorEvents>
  </xsl:variable>

  <xsl:template match="ErrorEvents">
    <h2 style="color:#ff0000">
      Conversion Failed:
    </h2>
    <table cellpadding="2" cellspacing="0" width="98%" border="1" bordercolor="white" class="infotable">
      <tr>
        <td nowrap="1" class="header">
          Error
        </td>
      </tr>
      <xsl:for-each select="Error">
        <xsl:for-each select="Line">
          <tr>
            <td nowrap="1" class="content" style="color:#ff0000">
              <xsl:value-of select="@Message"/>
            </td>
          </tr>
        </xsl:for-each>
      </xsl:for-each>
    </table>
  </xsl:template>
  
  <xsl:variable name="ConversionItemEvents">
    <ItemEvents>
      <xsl:attribute name="ConversionFile">
        <xsl:value-of select="/UpgradeLog/Properties/Property[@Name='Conversion File']/@Value"/>
      </xsl:attribute>
      
      <xsl:for-each select="/UpgradeLog/Events/Event[count(. | key('SourceItemKey',@SourceItem)[1]) = 1 and @SourceType = 'ConversionFile']">
        <xsl:sort select="@SourceItem" order="descending"/>
        
          <GroupedItemEvents>

            <xsl:attribute name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:attribute>

            <xsl:variable name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:variable>

            <xsl:for-each select="key('SourceItemKey', $SourceItem)">
              
              <Event>
                <xsl:attribute name="Message">
                  <xsl:value-of select="@Message"/>
                </xsl:attribute>
                <xsl:attribute name="SourceItem">
                  <xsl:value-of select="@SourceItem"/>
                </xsl:attribute>
                <xsl:attribute name="EventType">
                  <xsl:value-of select="@EventType"/>
                </xsl:attribute>
              </Event>
            </xsl:for-each>


          </GroupedItemEvents>

      </xsl:for-each>
    </ItemEvents>
  </xsl:variable>
  
  <xsl:variable name="ProjectEvents">
    <ProjectEvents>
      <xsl:attribute name="Project">
        <xsl:value-of select="/UpgradeLog/Properties/Property[@Name='Project']/@Value"/>
      </xsl:attribute>
      
      <xsl:for-each select="/UpgradeLog/Events/Event[count(. | key('SourceItemKey',@SourceItem)[1]) = 1 and @SourceType = 'Project']">
        
        <xsl:sort select="@SourceItem" order="ascending"/>
        <xsl:if test="(1=position()) or (preceding-sibling::*[1]/@SourceItem != @SourceItem)">

          <GroupedProjectEvents>

            <xsl:attribute name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:attribute>

            <xsl:variable name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:variable>

            <xsl:for-each select="key('SourceItemKey', $SourceItem)[ @SourceItem = $SourceItem ]">
              <Event>
                <xsl:attribute name="Message">
                  <xsl:value-of select="@Message"/>
                </xsl:attribute>
                <xsl:attribute name="SourceItem">
                  <xsl:value-of select="@SourceItem"/>
                </xsl:attribute>
                <xsl:attribute name="EventType">
                  <xsl:value-of select="@EventType"/>
                </xsl:attribute>
              </Event>
            </xsl:for-each>


          </GroupedProjectEvents>

        </xsl:if>
      </xsl:for-each>
    </ProjectEvents>
  </xsl:variable>

  <xsl:template match="ItemEvents">
    <h2>
      <a>Conversion File</a>: <xsl:value-of select="@ConversionFile"/>
    </h2>
    <table cellpadding="2" cellspacing="0" width="98%" border="1" bordercolor="white" class="infotable">
      <tr>
        <td nowrap="1" class="header">Item</td>
        <td nowrap="1" class="header" width="13"/>
        <td nowrap="1" class="header">Status</td>
        <td nowrap="1" class="header">Actions</td>
        <td nowrap="1" class="header">Errors</td>
        <td nowrap="1" class="header">Warnings</td>
      </tr>
      <xsl:for-each select="GroupedItemEvents">
        <xsl:variable name="source-id" select="generate-id(.)"/>
        <xsl:variable name="eventsCount" select="count(Event)"/>
        <xsl:variable name="successesCount" select="count(Event[@EventType='Success'])"/>
        <xsl:variable name="errorsCount" select="count(Event[@EventType = 'Error'])"/>
        <xsl:variable name="warningsCount" select="count(Event[@EventType = 'Warning'])"/>
        <tr class="row">
          <td class="content"><A HREF="javascript:"><xsl:attribute name="onClick">javascript:document.images['<xsl:value-of select="$source-id"/>'].click()</xsl:attribute><IMG border="0" _locID="IMG.alt" _locAttrData="alt"  alt="expand/collapse section" class="expandable" height="11" onclick="changepic()" src="_EFConversionWizard_Files/ConversionWizard_Plus.gif" width="9" ><xsl:attribute name="name"><xsl:value-of select="$source-id"/></xsl:attribute><xsl:attribute name="child">src<xsl:value-of select="$source-id"/></xsl:attribute></IMG></A>&#32;<xsl:value-of select="@SourceItem"/>
          </td>
          <td class="content" width="13">
            <xsl:if test="$successesCount = $eventsCount"><IMG src="_EFConversionWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$errorsCount > 0"><IMG src="_EFConversionWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$warningsCount > 0"><IMG src="_EFConversionWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>            
          </td>
          <td class="content">
            <xsl:if test="$successesCount = $eventsCount"><a>Converted</a></xsl:if>
            <xsl:if test="$errorsCount > 0"><a>Converted with errors</a></xsl:if>
            <xsl:if test="$warningsCount > 0"><a>Converted with warnings</a></xsl:if>
          </td>
          <td class="content"><xsl:value-of select="$eventsCount"/></td>
          <td class="content"><xsl:value-of select="$errorsCount"/></td>
          <td class="content"><xsl:value-of select="$warningsCount"/></td>
        </tr>
        
        <tr class="collapsed" bgcolor="#ffffff">
            <xsl:attribute name="id">src<xsl:value-of select="$source-id"/></xsl:attribute>

            <td colspan="7">
              <table width="97%" border="1" bordercolor="#dcdcdc" rules="cols" class="issuetable">
                <tr>
                  <td colspan="7" class="issuetitle">
                    Conversion Report - <xsl:value-of select="@SourceItem"/>:
                  </td>
                </tr>
                  <xsl:for-each select="Event">
                    <tr>
                      <td class="issuenone" style="border-bottom:solid 1 lightgray">
                        <xsl:if test="@EventType = 'Success'"><IMG src="_EFConversionWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Error'"><IMG src="_EFConversionWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Warning'"><IMG src="_EFConversionWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>            
                        &#32;&#32;<xsl:value-of select="@Message"/>
                      </td>
                    </tr>
                  </xsl:for-each>
               
              </table>
            </td>
          </tr>
        
      </xsl:for-each>
    </table>
  </xsl:template>
  
  <xsl:template match="ProjectEvents">
    <h2>
      <a>Project</a>: <xsl:value-of select="@Project"/>
    </h2>
    <table cellpadding="2" cellspacing="0" width="98%" border="1" bordercolor="white" class="infotable">
      <tr>
        <td nowrap="1" class="header">File</td>
        <td nowrap="1" class="header" width="13"/>
        <td nowrap="1" class="header">Status</td>
        <td nowrap="1" class="header">Actions</td>
        <td nowrap="1" class="header">Errors</td>
        <td nowrap="1" class="header">Warnings</td>
      </tr>
      <xsl:for-each select="GroupedProjectEvents">
        <xsl:sort select="count(Event)" order="descending"/>
        <xsl:variable name="source-id" select="generate-id(.)"/>
        <xsl:variable name="eventsCount" select="count(Event)"/>
        <xsl:variable name="successesCount" select="count(Event[@EventType='Success'])"/>
        <xsl:variable name="errorsCount" select="count(Event[@EventType = 'Error'])"/>
        <xsl:variable name="warningsCount" select="count(Event[@EventType = 'Warning'])"/>
        <tr class="row">
          <td class="content"><A HREF="javascript:"><xsl:attribute name="onClick">javascript:document.images['<xsl:value-of select="$source-id"/>'].click()</xsl:attribute><IMG border="0" _locID="IMG.alt" _locAttrData="alt"  alt="expand/collapse section" class="expandable" height="11" onclick="changepic()" src="_EFConversionWizard_Files/ConversionWizard_Plus.gif" width="9" ><xsl:attribute name="name"><xsl:value-of select="$source-id"/></xsl:attribute><xsl:attribute name="child">src<xsl:value-of select="$source-id"/></xsl:attribute></IMG></A>&#32;<xsl:value-of select="@SourceItem"/>
          </td>
          <td class="content" width="13">
            <xsl:if test="$successesCount = $eventsCount"><IMG src="_EFConversionWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$errorsCount > 0"><IMG src="_EFConversionWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$warningsCount > 0"><IMG src="_EFConversionWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>            
          </td>
          <td class="content">
            <xsl:choose>
              <xsl:when test="count(Event) = 1">
                <a>Backed up</a>
              </xsl:when>
              <xsl:otherwise>
                <a>Modified and backed up</a>
              </xsl:otherwise>
            </xsl:choose>
          </td>
          <td class="content"><xsl:value-of select="count(Event)"/></td>
          <td class="content"><xsl:value-of select="count(Event[@EventType = 'Error'])"/></td>
          <td class="content"><xsl:value-of select="count(Event[@EventType = 'Warning'])"/></td>
        </tr>
        
        <tr class="collapsed" bgcolor="#ffffff">
            <xsl:attribute name="id">src<xsl:value-of select="$source-id"/></xsl:attribute>

            <td colspan="7">
              <table width="97%" border="1" bordercolor="#dcdcdc" rules="cols" class="issuetable">
                <tr>
                  <td colspan="7" class="issuetitle">
                    Conversion Report - <xsl:value-of select="@SourceItem"/>:
                  </td>
                </tr>
                  <xsl:for-each select="Event">
                    <tr>
                      <td class="issuenone" style="border-bottom:solid 1 lightgray">
                        <xsl:if test="@EventType = 'Success'"><IMG src="_EFConversionWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Error'"><IMG src="_EFConversionWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Warning'"><IMG src="_EFConversionWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>            
                        &#32;&#32;<xsl:value-of select="@Message"/>
                      </td>
                    </tr>
                  </xsl:for-each>
               
              </table>
            </td>
          </tr>
        
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="Property">
    <xsl:if test="@Name!='Date' and @Name!='Time' and @Name!='Project' and @Name!='Solution'">
      <tr>
        <td nowrap="1">
          <b>
            <xsl:value-of select="@Name"/>:
          </b>
          <xsl:value-of select="@Value"/>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="UpgradeLog">
    <html>
      <head>
        <META HTTP-EQUIV="Content-Type" content="text/html; charset=utf-8" />
        <link rel="stylesheet" href="_EFConversionWizard_Files\ConversionWizardLog.css" />
        <title>
          Conversion Report
        </title>
        <script language="javascript">
          function outliner () {
          oMe = window.event.srcElement
          //get child element
          var child = document.all[event.srcElement.getAttribute("child",false)];
          //if child element exists, expand or collapse it.
          if (null != child)
          child.className = child.className == "collapsed" ? "expanded" : "collapsed";
          }

          function changepic() {
          uMe = window.event.srcElement;
          var check = uMe.src.toLowerCase();
          if (check.lastIndexOf("conversionwizard_plus.gif") != -1)
          {
          uMe.src = "_EFConversionWizard_Files/ConversionWizard_Minus.gif"
          }
          else
          {
          uMe.src = "_EFConversionWizard_Files/ConversionWizard_Plus.gif"
          }
          }
        </script>
      </head>
      <body topmargin="0" leftmargin="0" rightmargin="0" onclick="outliner();">
        <h1>
          Conversion Report - <xsl:value-of select="Properties/Property[@Name='Project']/@Value"/>
        </h1>
        <p>
          <table class="note">
            <tr>
              <td nowrap="1">
                <b>Conversion Settings</b>
              </td>
            </tr>
            <tr>
              <td nowrap="1">
                <b>Time of Conversion:</b>&#32;&#32;<xsl:value-of select="Properties/Property[@Name='Date']/@Value"/>&#32;&#32;<xsl:value-of select="Properties/Property[@Name='Time']/@Value"/><br/>
              </td>
            </tr>
            <xsl:apply-templates select="Properties"/>
          </table>
        </p>

        <xsl:choose>
          <xsl:when test="count(Errors/Error) > 0">
            <xsl:apply-templates select="msxsl:node-set($ErrorEvents)/*"/>    
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates select="msxsl:node-set($Hints)/*"/>
          </xsl:otherwise>
        </xsl:choose>
        
        <xsl:apply-templates select="msxsl:node-set($ConversionItemEvents)/*"/>
        <xsl:apply-templates select="msxsl:node-set($ProjectEvents)/*"/>
                
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>