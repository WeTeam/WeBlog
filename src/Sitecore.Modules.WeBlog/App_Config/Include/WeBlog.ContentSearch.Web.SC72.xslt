<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-web']/param[@desc='propertyStore']/@ref">
    <xsl:attribute name="ref">contentSearch/databasePropertyStore</xsl:attribute>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-web']/strategies/strategy[@ref='contentSearch/indexConfigurations/indexUpdateStrategies/rebuildAfterFullPublish']/@ref">
    <xsl:attribute name="ref">contentSearch/indexUpdateStrategies/rebuildAfterFullPublish</xsl:attribute>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-web']/strategies/strategy[@ref='contentSearch/indexConfigurations/indexUpdateStrategies/onPublishEndAsync']/@ref">
    <xsl:attribute name="ref">contentSearch/indexUpdateStrategies/onPublishEndAsync</xsl:attribute>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-web']/strategies/strategy[@ref='contentSearch/indexConfigurations/indexUpdateStrategies/remoteRebuild']/@ref">
    <xsl:attribute name="ref">contentSearch/indexUpdateStrategies/remoteRebuild</xsl:attribute>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
