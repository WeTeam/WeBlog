<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>
  <xsl:param name="custom-cache-clear" select="true()"/>

  <xsl:template match="/configuration/sitecore/events">
    <events>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="$custom-cache-clear">
        <!-- Clear HTML caches after index update is complete. Add any additional WeBlog sites to the site list below (or via patch). -->
        <event name="database:propertychanged">
          <handler type="Sitecore.Modules.WeBlog.Search.IndexUpdateHtmlCacheClearer, Sitecore.Modules.WeBlog" method="OnPropertyChanged">
            <sites hint="list">
              <site>website</site>
            </sites>
          </handler>
        </event>
      </xsl:if>
    </events>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/events/event[@name='item:saved']/handler[@type='Sitecore.Modules.WeBlog.EventHandlers.SyncBucket, Sitecore.Modules.WeBlog']"/>

  <xsl:template match="/configuration/sitecore/settings/setting[@name='WeBlog.CommentService.Enable']/@value">
    <xsl:attribute name="value">true</xsl:attribute>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:newblog']"/>
  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:newentry']"/>
  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:newcategory']"/>
  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:blogsettings']"/>
  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:entrysettings']"/>
  <xsl:template match="/configuration/sitecore/commands/command[@name='blog:import']"/>


  <xsl:template match="/configuration/sitecore/search/configuration/indexes/index[@id='WeBlog']/locations/master"/>
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
