<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0"
                        exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime"
                        xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer"
                        xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:sppchemas-microsoft-com:xslt"
                        xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" ddwrt:oob="true">

    <xsl:include href="/_layouts/xsl/main.xsl"/>
    <xsl:include href="/_layouts/xsl/internal.xsl"/>

    <xsl:template name="Freeform">
        <xsl:param name="AddNewText"/>
        <xsl:param name="ID"/>
        <xsl:variable name="Url">
            <xsl:choose>
                <xsl:when test="List/@TemplateType='119'">
                    <xsl:value-of select="$HttpVDir"/>/_layouts/CreateWebPage.aspx?List=<xsl:value-of select="$List"/>&amp;RootFolder=<xsl:value-of select="$XmlDefinition/List/@RootFolder"/>
                </xsl:when>
                <xsl:when test="$IsDocLib">
                    <xsl:value-of select="$HttpVDir"/>/_layouts/LargeFileUploadWithSLToSP/LibraryUpload.aspx?documentLibraryId=<xsl:value-of select="$List"/>&amp;RootFolder=<xsl:value-of select="$XmlDefinition/List/@RootFolder"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$ENCODED_FORM_NEW"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="HeroStyle">
            <xsl:choose>
                <xsl:when test="Toolbar[@Type='Standard']">display:none</xsl:when>
                <xsl:otherwise></xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:if test="$ListRight_AddListItems = '1' and (not($InlineEdit) or $IsDocLib)">
            <table id="Hero-{$WPQ}" width="100%" cellpadding="0" cellspacing="0" border="0" style="{$HeroStyle}">
                <tr>
                    <td colspan="2" class="ms-partline">
                        <img src="/_layouts/images/blank.gif" width="1" height="1" alt="" />
                    </td>
                </tr>
                <tr>
                    <td class="ms-addnew" style="padding-bottom: 3px">
                        <span style="height:10px;width:10px;position:relative;display:inline-block;overflow:hidden;" class="s4-clust">
                            <img src="/_layouts/images/fgimg.png" alt="" style="left:-0px !important;top:-128px !important;position:absolute;"  />
                        </span>
                        <xsl:text disable-output-escaping="yes" ddwrt:nbsp-preserve="yes">&amp;nbsp;</xsl:text>
                        <xsl:choose>
                            <xsl:when test="List/@TemplateType = '115'">
                                <a class="ms-addnew" id="{$ID}-{$WPQ}"
                                   href="{$Url}"
                                   onclick="javascript:NewItem2(event, &quot;{$Url}&quot;);javascript:return false;"
                                   target="_self">
                                    <xsl:value-of select="$AddNewText" />
                                </a>
                            </xsl:when>
                            <xsl:otherwise>
                                <a class="ms-addnew" id="{$ID}"
                                   href="{$Url}"
                                   onclick="javascript:NewItem2(event, &quot;{$Url}&quot;);javascript:return false;"
                                   target="_self">
                                    <xsl:value-of select="$AddNewText" />
                                </a>
                            </xsl:otherwise>
                        </xsl:choose>
                    </td>
                </tr>
                <tr>
                    <td>
                        <img src="/_layouts/images/blank.gif" width="1" height="5" alt="" />
                    </td>
                </tr>
            </table>
            <xsl:choose>
                <xsl:when test="Toolbar[@Type='Standard']">
                    <script type='text/javascript'>
                        if (typeof(heroButtonWebPart<xsl:value-of select="$WPQ"/>) != "undefined")
                        {
                        <xsl:value-of select="concat('  var eleHero = document.getElementById(&quot;Hero-', $WPQ, '&quot;);')"/>
                        if (eleHero != null)
                        eleHero.style.display = "";
                        }
                    </script>
                </xsl:when>
                <xsl:otherwise>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:if test="List/@TemplateType = '115'">
                <script type='text/javascript'>
                    if (typeof(DefaultNewButtonWebPart<xsl:value-of select="$WPQ"/>) != "undefined")
                    {
                    <xsl:value-of select="concat('  var eleLink = document.getElementById(&quot;', $ID, '-', $WPQ, '&quot;);')"/>
                    if (eleLink != null)
                    {
                    DefaultNewButtonWebPart<xsl:value-of select="$WPQ"/>(eleLink);
                    }
                    }
                </script>
            </xsl:if>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>